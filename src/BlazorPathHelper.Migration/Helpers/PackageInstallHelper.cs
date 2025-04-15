using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ZLogger;
namespace BlazorPathHelper.Migration.Helpers;

internal class PackageInstallHelper(
    ILogger<PackageInstallHelper> logger
)
{
    private const string PackageName = "BlazorPathHelper";

    /// <summary>
    /// Installs the BlazorPathHelper package to the specified project directory.
    /// </summary>
    public async Task<bool> InstallBlazorPathHelperToProject(string projectDir)
    {
        var projectFile = Directory.EnumerateFiles(projectDir, "*.csproj", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();
        if (projectFile == null)
        {
            throw new FileNotFoundException("Project file not found", projectDir);
        }
        var projectFullPath = Path.GetFullPath(projectFile);
        var projectDirectory = Path.GetDirectoryName(projectFullPath);
        var projectName = Path.GetFileNameWithoutExtension(projectFile);

        logger.ZLogInformation($"Installing {PackageName} to {projectName} ...");

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"add package {PackageName}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = projectDirectory,
        };
        var process = await StartProcessWithCancellation(processStartInfo);

        if (process?.ExitCode == 0)
        {
            logger.ZLogInformation($"Successfully installed!");
            return true;
        }
        else
        {
            logger.ZLogError($"Failed to install. try manually run `dotnet add package {PackageName}`.");
            return false;
        }
    }

    private async Task<Process?> StartProcessWithCancellation(ProcessStartInfo processStartInfo)
    {
        using var process = new Process
        {
            StartInfo = processStartInfo,
        };
        process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

        void canceled(object? sender, ConsoleCancelEventArgs e)
        {
            if (!process.HasExited)
            {
                process.Kill();
                logger.ZLogInformation($"Process was terminated by Ctrl+C.");
            }
            e.Cancel = true;
        }

        Console.CancelKeyPress += canceled;
        logger.ZLogInformation($"Install Process can be canceled by Ctrl+C.");
        process.Start();
        await process.WaitForExitAsync();
        Console.CancelKeyPress -= canceled;
        logger.ZLogInformation($"Install Process was finished.");
        return process;
    }
}
