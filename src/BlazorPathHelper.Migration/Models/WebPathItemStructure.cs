using Microsoft.CodeAnalysis.CSharp;

namespace BlazorPathHelper.Migration.Models;

internal class WebPathItemStructure
{
    public required SourceFileData Source { get; init; }
    public required string Path { get; init; }
    public required string VariableName
    {
        get => _variableName;
        init => _variableName = UsableVariableName(value);
    }
    public required string ComponentFullName { get; init; }

    private string _variableName = string.Empty;

    public SyntaxKind Accessibility { get; init; } = SyntaxKind.PublicKeyword;
    public List<WebPathQueryStructure> QueryParameters { get; init; } = [];

    public string ComponentName => ComponentFullName.Split('.').Last();
    public bool HasQueryParameters => QueryParameters.Count > 0;

    /// <summary>
    /// Generates a variable name for the web path item, ensuring no conflicts with existing variable names.
    /// </summary>
    public string NoConflictVariableName(IEnumerable<WebPathItemStructure> webPaths)
    {
        // find same variablename from webpaths
        var sameVariableName = webPaths
            .Where(w => w.ComponentFullName != ComponentFullName)
            .Where(w => w.VariableName == VariableName);
        // if no same variablename, return the original
        if (!sameVariableName.Any())
        {
            return VariableName;
        }
        // if same variable name exists, use fullname
        return ComponentFullName.Replace(".", "_");
    }

    private string UsableVariableName(string variableName)
    {
        // remove special characters
        var usableVariableName = variableName.Replace(" ", "_").Replace("-", "_").Replace(".", "_");
        // remove leading and trailing underscores
        usableVariableName = usableVariableName.Trim('_');
        return usableVariableName;
    }
}
