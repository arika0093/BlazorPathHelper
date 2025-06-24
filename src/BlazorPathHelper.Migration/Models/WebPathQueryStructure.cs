namespace BlazorPathHelper.Migration.Models;

/// <summary>
/// This class represents a query parameter structure.
/// </summary>
internal class WebPathQueryStructure
{
    public required string QueryName { get; init; }
    public required string Type { get; init; }
    public required string VariableName { get; init; }
    public string? DefaultValue { get; init; }
}
