using System.Collections.Generic;
using System.Linq;

namespace BlazorPathHelper.Models;

/// <summary>
/// Factory class responsible for creating hierarchical tree structures from parse records.
/// These structures are used to represent the relationship between different path components
/// in the Blazor application's routing system.
/// </summary>
internal static class ParseRecordTreeStructureFactory
{
    /// <summary>
    /// create record tree structure.
    /// </summary>
    /// <param name="records">records</param>
    /// <returns>record tree structure</returns>
    public static List<ParseRecordTreeStructure> Create(List<ParseRecord> records)
    {
        var usableRecords = records.Where(b => b.IsDisplay).ToList();
        var rootItems = usableRecords.Where(b => b.IsRoot);
        return rootItems.Select(r => CreateTreeRecord(usableRecords, r)).ToList();
    }

    /// <summary>
    /// create record tree structure.
    /// </summary>
    /// <param name="allRecords">List of all available records</param>
    /// <param name="rootRecord">The root record to create tree from</param>
    /// <returns></returns>
    private static ParseRecordTreeStructure CreateTreeRecord(List<ParseRecord> allRecords, ParseRecord rootRecord)
    {
        var rootIndex = allRecords.FindIndex(r => r.PathRawValue == rootRecord.PathRawValue);
        var childItems = allRecords
            .Where(r => !r.IsRoot)
            .Where(r => r.GroupPath == rootRecord.PathRawValue.TrimEnd('/'))
            // prevent infinite loop
            .Where(r => r.PathRawValue != rootRecord.PathRawValue)
            .Select(r => CreateTreeRecord(allRecords, r))
            .ToArray();
        return new()
        {
            Index = rootIndex,
            Record = rootRecord,
            ChildItems = childItems
        };
    }
}