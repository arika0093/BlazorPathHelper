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
            WorkingDirectory = projectDirectory,
        };

        using var process = Process.Start(processStartInfo);
        if (process == null)
        {
            logger.ZLogError($"Failed to start process to install.");
            return false;
        }

        await process.WaitForExitAsync();

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
}
