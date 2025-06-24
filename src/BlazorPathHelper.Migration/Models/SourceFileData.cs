namespace BlazorPathHelper.Migration.Models;

/// <summary>
/// This class stores the parsed data of a source file.
/// </summary>
internal class SourceFileData
{
    public required ParsedFileType FileType { get; init; }
    public required string FilePath { get; init; }
    public required string FileContent { get; init; }
}

internal enum ParsedFileType
{
    Razor,
    Csharp,
    // CsHtml
}
