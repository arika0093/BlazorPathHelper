using Microsoft.Extensions.Logging;
using Sharprompt;
using ZLogger;

namespace BlazorPathHelper.Migration.Helpers;

internal class ProjectSelectorHelper(
    ILogger<ProjectSelectorHelper> logger
)
{
    /// <summary>
    /// Convert project file paths to directory paths.
    /// </summary>
    public string[] ConvertProjectFileToDirectory(string[]? projects)
    {
        if (projects == null || projects.Length == 0)
        {
            return [];
        }
        var directories = new List<string>();
        foreach (var project in projects)
        {
            if (Directory.Exists(project))
            {
                directories.Add(project);
            }
            else if (Path.GetExtension(project) == ".csproj")
            {
                var directory = Path.GetDirectoryName(project);
                if (directory != null && !directories.Contains(directory))
                {
                    directories.Add(directory);
                }
            }
        }
        return [.. directories];
    }

    /// <summary>
    /// select projects from the specified directory.
    /// </summary>
    public string[] SelectProjectsFromCurrentDirs()
    {
#if DEBUG
        // for debugging purposes, set the current directory to the root directory of the project.
        var currentDirectory = "..\\..\\..\\..\\..\\";
#else
        var currentDirectory = Environment.CurrentDirectory;
#endif
        logger.ZLogDebug($"Current directory: {currentDirectory}");
        return SelectProjectsFromSpecifiedDirs(currentDirectory) ?? [];
    }

    /// <summary>
    /// select projects from the root directory.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private string[] SelectProjectsFromSpecifiedDirs(string rootDir)
    {
        var csprojDirs = ExtractCsprojDirectories(rootDir);
        if (csprojDirs == null || csprojDirs.Length == 0)
        {
            logger.ZLogError($"No csproj files found in the directory.");
            return [];
        }
        var selects = Prompt.MultiSelect(new MultiSelectOptions<FileLocation>()
        {
            Message = "Select csproj files to migrate",
            PageSize = 10,
            Items = csprojDirs,
            Minimum = 1,
            TextSelector = x => $"{x.FileName} ({x.RelativePath(rootDir)})",
        });
        if (selects == null || !selects.Any())
        {
            logger.ZLogError($"No csproj files selected.");
            return [];
        }
        return [.. selects.Select(x => x.Directory)];
    }

    /// <summary>
    /// find csproj existed directories from the root directory.
    /// </summary>
    private static FileLocation[]? ExtractCsprojDirectories(string rootDir)
    {
        var csprojFiles = Directory.EnumerateFiles(rootDir, "*.csproj", SearchOption.AllDirectories);
        if (csprojFiles == null || !csprojFiles.Any())
        {
            return null;
        }
        return [.. csprojFiles.Select(x => new FileLocation(x))];
    }
}

// This class is used to represent a file location.
internal record FileLocation(string CsProjPath)
{
    public string FilePath => Path.GetFullPath(CsProjPath) ?? string.Empty;
    public string FileName => Path.GetFileName(CsProjPath) ?? string.Empty;
    public string Directory => Path.GetDirectoryName(CsProjPath) ?? string.Empty;
    public string RelativePath(string path) => Path.GetRelativePath(path, Directory);
}