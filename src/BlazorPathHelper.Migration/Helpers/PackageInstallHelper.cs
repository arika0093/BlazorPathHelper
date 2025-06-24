using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace BlazorPathHelper.Migration.Helpers;

internal class PackageInstallHelper(ILogger<PackageInstallHelper> logger)
{
    private const string PackageName = "BlazorPathHelper";

    /// <summary>
    /// Installs the BlazorPathHelper package to the specified project directory.
    /// </summary>
    public async Task<bool> InstallBlazorPathHelperToProject(string projectDir)
    {
        var projectFile = Directory
            .EnumerateFiles(projectDir, "*.csproj", SearchOption.TopDirectoryOnly)
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
        return await StartProcessWithCancellation(processStartInfo);
    }

    /// <summary>
    /// Starts a process with cancellation support.
    /// </summary>
    private async Task<bool> StartProcessWithCancellation(ProcessStartInfo processStartInfo)
    {
        using var process = new Process { StartInfo = processStartInfo };

        void canceled(object? sender, ConsoleCancelEventArgs e)
        {
            if (!process.HasExited)
            {
                process.Kill();
                logger.ZLogInformation($"Process was terminated.");
            }
            e.Cancel = true;
        }

        Console.CancelKeyPress += canceled;
        logger.ZLogInformation($"Install Process can be canceled by Ctrl+C.");
        process.Start();
        await process.WaitForExitAsync();
        Console.CancelKeyPress -= canceled;
        logger.ZLogInformation($"Install Process was finished.");

        if (process != null)
        {
            var stdout = await process.StandardOutput.ReadToEndAsync();
            logger.ZLogTrace($"{stdout}");
        }
        if (process?.ExitCode == 0)
        {
            logger.ZLogInformation($"Successfully installed!");
            return true;
        }
        else
        {
            logger.ZLogError(
                $"Failed to install. try manually run `dotnet add package {PackageName}`."
            );
            return false;
        }
    }
}
