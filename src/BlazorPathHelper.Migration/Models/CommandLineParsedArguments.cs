namespace BlazorPathHelper.Migration.Models;


/// <summary>
/// This record stores the parsed results of the command-line arguments.
/// </summary>
internal record CommandLineParsedArguments
{
    /// <summary>
    /// Paths to the target projects.
    /// </summary>
    public required string ProjectPath { get; init; }

    /// <summary>
    /// Whether to replace the @page attribute string with a generated variable.
    /// </summary>
    public required bool IsReplacePageAttributeString { get; init; }

    /// <summary>
    /// Whether to generate [Query] attributes from [SupportQueryBuilder] values.
    /// </summary>
    public required bool QueryBuilderSupport { get; init; }

    /// <summary>
    /// The name of the generated class.
    /// </summary>
    public required string OutputClassName { get; init; } 

    /// <summary>
    /// The output directory for the generated code.
    /// </summary>
    public required string OutputDir { get; init; }

    /// <summary>
    /// Whether to overwrite existing files in the output path.
    /// </summary>
    public required bool ForceExport { get; init; }

    /// <summary>
    /// Whether to disable interactive mode.
    /// </summary>
    public required bool DisableInteractiveMode { get; init; }

    /// <summary>
    /// Whether to perform a dry run (no code generation).
    /// </summary>
    public required bool IsDryRun { get; init; } = false;

    /// <summary>
    /// The full path to the output file.
    /// </summary>
    public string OutputFileFullPath
        => Path.Combine(ProjectPath, OutputDir, $"{OutputClassName}.cs");

    /// <summary>
    /// Returns a string representation of the command-line arguments.
    /// </summary>
    public override string ToString()
    {
        return $"ProjectPath: {ProjectPath}, " +
               $"IsReplacePageAttributeString: {IsReplacePageAttributeString}, " +
               $"QueryBuilderSupport: {QueryBuilderSupport}, " +
               $"OutputPath: {OutputDir}, " +
               $"ForceExport: {ForceExport}, " +
               $"DisableInteractiveMode: {DisableInteractiveMode}, " +
               $"IsDryRun: {IsDryRun}";
    }
}