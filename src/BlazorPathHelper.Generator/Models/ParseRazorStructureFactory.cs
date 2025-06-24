using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.Models;

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
                // get @page or Route["..."] from source use regex
                // see: https://regex101.com/r/8t229K/2
                List<string> pagePaths = [];
                var pageRegex = new Regex(@"@page\s+""(.+)""|@attribute\s+\[.*Route(?:Attribute)?\(""(.+)""\)\]");
                var pageMatch = pageRegex.Matches(source?.ToString() ?? "");
                if(pageMatch.Count > 0)
                {
                    foreach (Match match in pageMatch)
                    {
                        // get page path from match
                        var pagePath = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                        // add to list
                        if (!string.IsNullOrWhiteSpace(pagePath))
                        {
                            pagePaths.Add(pagePath);
                        }
                    }
                }

                return new ParseRazorStructure()
                {
                    ProjectDirectory = p.ProjectDirectory,
                    DefaultNamespace = p.ProjectNamespace,
                    FullPath = filePath,
                    PageClassName = fileName,
                    PagePaths = pagePaths,
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