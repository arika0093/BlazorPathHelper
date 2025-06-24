namespace BlazorPathHelper.Models;

/// <summary>
/// Parse record tree structure.
/// </summary>
internal class ParseRecordTreeStructure
{
    /// <summary>
    /// index of this record.
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// menu record.
    /// </summary>
    public required ParseRecord Record { get; init; }

    /// <summary>
    /// child items.
    /// </summary>
    public required ParseRecordTreeStructure[] ChildItems { get; init; }
}
