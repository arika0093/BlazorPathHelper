using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
// ReSharper disable StringLiteralTypo

namespace BlazorPathHelper.Models;

/// <summary>
/// stored information for razor file.
/// </summary>
internal record ParseRazorStructure
{
    /// <summary>
    /// project directory of razor file.
    /// </summary>
    public required string ProjectDirectory { get; init; }

    /// <summary>
    /// default namespace of project.
    /// </summary>
    public required string DefaultNamespace { get; init; }

    /// <summary>
    /// full path of razor file.
    /// </summary>
    public required string FullPath { get; init; }

    /// <summary>
    /// namespace of razor file.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// class name of razor file.
    /// </summary>
    public required string PageClassName { get; init; }
}

/// <summary>
/// factory for ParseRazorStructure.
/// </summary>
internal static class ParseRazorStructureFactory
{
    /// <summary>
    /// parse razor files and return ParseRazorStructure.
    /// </summary>
    public static IncrementalValueProvider<ImmutableArray<ParseRazorStructure>> ParseRazorFiles(
        IncrementalGeneratorInitializationContext context)
    {
        return context.AdditionalTextsProvider
            // open all razor files.
            .Where(static i =>
                i.Path.EndsWith(".razor", StringComparison.OrdinalIgnoreCase) || 
                i.Path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            // and concat project information (for get namespace and project directory)
            .Combine(context.AnalyzerConfigOptionsProvider)
            // parse razor file and get information.
            .Select((pair, _) =>
            {
                var globalOpt = pair.Right.GlobalOptions;
                globalOpt.TryGetValue("build_property.rootnamespace", out var projectNamespace);
                globalOpt.TryGetValue("build_property.projectdir", out var projectDirectory);
                return new
                {
                    Text = pair.Left,
                    ProjectDirectory = projectDirectory!,
                    ProjectNamespace = projectNamespace!,
                };
            })
            // create ParseRazorStructure.
            .Select((p, _) =>
            {
                var filePath = p.Text.Path;
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var source = p.Text.GetText();
                // get default namespace.
                // (There is a possibility that it is slow to compile razor files, so guess from file name and project namespace.)
                var relativeNs = GetRelativeNamespace(p.ProjectDirectory, filePath, p.ProjectNamespace);
                // get namespace from source use regex
                // (it is pseudo-extracted with regular expressions.)
                var nsRegex = new Regex(@"@namespace\s+(?<namespace>[\w\.]+)");
                var nsMatch = nsRegex.Match(source?.ToString() ?? "");
                var ns = nsMatch.Success ? nsMatch.Groups["namespace"].Value : relativeNs;

                return new ParseRazorStructure()
                {
                    ProjectDirectory = p.ProjectDirectory,
                    DefaultNamespace = p.ProjectNamespace,
                    FullPath = filePath,
                    PageClassName = fileName,
                    Namespace = ns,
                };
            })
            .Collect();
    }

    // get relative namespace from root(project directory) and dest(.razor file path).
    // e.g.
    // defaultNamespace: "BlazorApp",
    // root: "C:\project\BlazorApp",
    // dest: "C:\project\BlazorApp\Pages\Index.razor"
    // -> "BlazorApp.Pages"
    private static string GetRelativeNamespace(string root, string dest, string defaultNamespace)
    {
        var rootFolderName = Path.GetDirectoryName(root) ?? "";
        var destFolderName = Path.GetDirectoryName(dest) ?? "";
        var relative = destFolderName
            .Replace(rootFolderName, "")
            .Replace(" ", "_")
            .Replace("/", ".")
            .Replace("\\", ".");
        return $"{defaultNamespace}{relative}";
    }
}