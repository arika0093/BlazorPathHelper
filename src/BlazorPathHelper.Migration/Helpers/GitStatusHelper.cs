using Microsoft.Extensions.Logging;
using ZLogger;

namespace BlazorPathHelper.Migration.Helpers;

internal class CheckGitStatusHelper(ILogger<CheckGitStatusHelper> logger)
{
    /// <summary>
    /// Checks if the current git status is clean (no uncommitted changes).
    /// </summary>
    public bool IsGitStatusClean(string targetDir)
    {
        try
        {
            if (!RunGitCommand(targetDir, "rev-parse --is-inside-work-tree").Contains("true"))
            {
                logger.ZLogWarning(
                    $"current directory is not a git repository. recommend to use git repository to rollback changes."
                );
                return false;
            }
            var gitStatus = RunGitCommand(targetDir, "status --porcelain");
            var isClean = string.IsNullOrWhiteSpace(gitStatus);
            if (!isClean)
            {
                logger.ZLogWarning(
                    $"Git status is not clean. Please commit or stash your changes."
                );
            }
            return isClean;
        }
        catch (Exception)
        {
            logger.ZLogError($"Error checking git status.");
            return false;
        }
    }

    private string RunGitCommand(string pwd, string command)
    {
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "git",
            Arguments = command,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = pwd,
        };
        using var process = System.Diagnostics.Process.Start(processStartInfo);
        using var reader = process?.StandardOutput;
        return reader?.ReadToEnd() ?? string.Empty;
    }
}
