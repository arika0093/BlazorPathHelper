namespace BlazorPathHelper.Models;

/// <summary>
/// argument information for builder method.
/// </summary>
/// <remarks>
/// e.g. path = "/sample/{value:int?}" so, VariableName = "value", Type = "int", IsNullable = true
/// </remarks>
internal record ParseParameterRecord
{
    /// <summary>
    /// name of variable. {value:int} -> value
    /// </summary>
    public required string VariableName { get; init; }

    /// <summary>
    /// type of variable. {value:int} -> int
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// is nullable or not. {value:int?} -> true
    /// </summary>
    public required bool IsNullable { get; init; }

    /// <summary>
    /// is catch all or not. {*value:int} -> true
    /// </summary>
    public required bool IsCatchAll { get; init; }

    /// <summary>
    /// argument definition for builder method.
    /// e.g. {value1} -> "string value1"
    /// e.g. {value2:int?} -> "int? value2 = null"
    /// </summary>
    public string ArgDefinition => $"{Type}{NullChar} {VariableName}{(IsNullable ? " = null" : "")}";

    /// <summary>
    /// argument definition for builder method.
    /// e.g. {value1} -> string.Format("{0}", value1)
    /// </summary>
    /// <see cref="BlazorPathHelperUtility">ToStringForUrl</see>
    public string VariableString => $"ToStringForUrl({VariableName})";

    private string NullChar => IsNullable ? "?" : "";
}