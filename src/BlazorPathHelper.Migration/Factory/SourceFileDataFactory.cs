using BlazorPathHelper.Migration.Models;

namespace BlazorPathHelper.Migration.Factory;

internal class SourceFileDataFactory
{
    /// <summary>
    /// Finds all source files in the specified project directory.
    /// </summary>
    public IEnumerable<SourceFileData> FindSources(string projectDir)
    {
        return FindRazorFiles(projectDir);
    }

    // find and read all razor files in the project directory
    private IEnumerable<SourceFileData> FindRazorFiles(string projectDir)
    {
        var razorFiles = Directory.EnumerateFiles(projectDir, "*.razor", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith(".g.cs") && !file.EndsWith(".g.i.cs"));
        foreach (var file in razorFiles)
        {
            yield return new SourceFileData
            {
                FileType = ParsedFileType.Razor,
                FilePath = file,
                FileContent = File.ReadAllText(file)
            };
        }
    }
}
