using BlazorPathHelper.Migration.Models;
using Microsoft.Extensions.Logging;
using Sharprompt;
using System.Text.RegularExpressions;
using ZLogger;

namespace BlazorPathHelper.Migration.Factory;

internal partial class WebPathItemFactory(
    ILogger<WebPathItemFactory> logger
)
{
    /// <summary>
    /// Generates a WebPathItemStructure from the given source file data.
    /// </summary>
    public IEnumerable<WebPathItemStructure> GenerateWebPathItem(
        SourceFileData source,
        CommandLineParsedArguments args)
    {
        return source.FileType switch
        {
            ParsedFileType.Razor => GenerateWebPathItemFromRazor(source, args),
            // ParsedFileType.Csharp => GenerateWebPathItemFromCsharp(source, args),
            _ => throw new NotImplementedException($"File type {source.FileType} is not supported.")
        };
    }

    /// <summary>
    /// Generates a WebPathItemStructure from Razor file data.
    /// </summary>
    private IEnumerable<WebPathItemStructure> GenerateWebPathItemFromRazor(
        SourceFileData source,
        CommandLineParsedArguments args)
    {
        // className be equal to the file name
        // e.g. Sample.razor -> Sample
        var className = Path.GetFileNameWithoutExtension(source.FilePath);
        // fullname be equal to the namespace + className
        // and namespace be equal to the relative path of the file
        // e.g. /foo/bar/Sample.razor -> foo.bar.Sample
        var relativePath = Path.GetRelativePath(args.ProjectPath, source.FilePath);
        var namespaceName = ConvertRelativePathToNamespace(relativePath);
        var fullClassName = !string.IsNullOrEmpty(namespaceName)
            ? $"{namespaceName}.{className}"
            : className;
        // @page "/test" -> test
        string pageString = string.Empty;
        var extractPageAttributeRegex = RazorPageAttrRegex();
        var pageAttrMatches = extractPageAttributeRegex.Matches(source.FileContent);
        if(pageAttrMatches.Count >= 2)
        {
            logger.ZLogWarning($"Multiple @page attributes found in {source.FilePath}");
            var selectedPageAttrMatch = Prompt.Select(new SelectOptions<Match>()
            {
                Message = $"Select @page attribute to use",
                Items = pageAttrMatches,
                DefaultValue = pageAttrMatches[0],
                TextSelector = (match) => match.Groups[1].Value,
            });
            pageString = selectedPageAttrMatch.Groups[1].Value;
        }
        else
        {
            var pstr = pageAttrMatches.FirstOrDefault()?.Groups[1].Value;
            if (string.IsNullOrEmpty(pstr))
            {
                yield break;
            }
            pageString = pstr;
        }
        // parse query parameters (SupplyParameterFromQuery)
        var queryParameters = GetWebPathQueryStructures(source, args);

        var rst = new WebPathItemStructure {
            Source = source,
            Path = pageString,
            VariableName = className,
            ComponentFullName = fullClassName,
            QueryParameters = [.. queryParameters],
        };
        yield return rst;
    }

    // Convert the relative path to a namespace by replacing directory separators with dots
    private static string ConvertRelativePathToNamespace(string relPath)
    {
        // for example:
        // /foo/bar/Sample.razor -> foo.bar.Sample
        // /foo/bar bar/Sample.razor -> foo.bar_bar.Sample
        // /foo/bar$$bar/Sample.razor -> foo.bar__bar.Sample
        var simplePathToNamespace = Path.GetDirectoryName(relPath)
            ?.Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');
        // and replace special characters with "_"
        var namespaceName = SpecialCharacterRegex().Replace(simplePathToNamespace ?? "", "_");
        return namespaceName;
    }

    private static IEnumerable<WebPathQueryStructure> GetWebPathQueryStructures(
        SourceFileData source,
        CommandLineParsedArguments args)
    {
        var matches = QueryBuilderRegex().Matches(source.FileContent);
        foreach (Match match in matches)
        {
            var queryName = match.Groups[1].Value;
            var queryType = match.Groups[2].Value;
            var queryVariableName = match.Groups[3].Value;
            var defaultValue = match.Groups[4].Value;
            // Check if the query variable name is empty
            if (string.IsNullOrEmpty(queryVariableName))
            {
                continue;
            }
            yield return new WebPathQueryStructure
            {
                QueryName = queryName,
                Type = queryType,
                VariableName = queryVariableName,
                DefaultValue = defaultValue,
            };
        }
    }

    // Group 1: Razor Page Attribute String
    // https://regex101.com/r/vVBxis/1
    [GeneratedRegex(@"@page\s+""(.*?)""", RegexOptions.IgnoreCase, "ja-JP")]
    private static partial Regex RazorPageAttrRegex();

    // Group 1: Query Variable Name in URL
    // Group 2: Query Variable Type
    // Group 3: Query Variable Name in C#
    // Group 4: Default Value
    // https://regex101.com/r/AWrD52/1
    [GeneratedRegex(@"\[\s*SupplyParameterFromQuery(?:Attribute)?\s*(?:\(Name\s*=\s*""(.+)""\))?\]\s+public\s+(.+?)\s+([^\s]+)\s*(?:{\s*get\s*;\s*(?:set|init)\s*;\s*})?(?:\s*=\s*([^\s;]+))?")]
    private static partial Regex QueryBuilderRegex();

    [GeneratedRegex(@"^[ -/:-@[-´{-~]*$")]
    private static partial Regex SpecialCharacterRegex();
}
