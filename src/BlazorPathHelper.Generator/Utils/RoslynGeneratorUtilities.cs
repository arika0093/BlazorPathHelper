using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorPathHelper.Utils;

internal static class RoslynGeneratorUtilities
{
    // get namespace string for use in code generation
    public static string? GetNamespace(INamespaceSymbol symbol)
    {
        // if global namespace, return null
        if (symbol.IsGlobalNamespace)
        {
            return null;
        }
        return symbol.ToDisplayString();
    }

    // get valid variable name for use in code generation
    public static string GetValidVariableName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }
        var pascalName = ToPascalCase(name);
        // if name will start with number, add underscore.
        if (char.IsDigit(pascalName[0]))
        {
            return $"_{pascalName}";
        }
        // if name is keyword, add at-mark.
        if (SyntaxFacts.GetKeywordKind(pascalName) != SyntaxKind.None)
        {
            return $"@{pascalName}";
        }
        return pascalName;
    }

    // get PascalCase string for use in code generation
    // thanks; https://stackoverflow.com/a/46095771
    public static string ToPascalCase(string name)
    {
        var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
        var whiteSpace = new Regex(@"(?<=\s)");
        var startsWithLowerCaseChar = new Regex("^[a-z]");
        var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
        var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
        var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

        var pascalCase = invalidCharsRgx
            .Replace(whiteSpace.Replace(name, "_"), string.Empty)
            .Split(['_'], StringSplitOptions.RemoveEmptyEntries)
            .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
            .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
            .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
            .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

        return string.Concat(pascalCase);
    }

    // get new line string for use in code generation
    public static string GetNewLine()
    {
        // Environment.NewLine is cannot use in code generation.
        // so use SyntaxFactory.ElasticCarriageReturnLineFeed.ToString() instead.
        return SyntaxFactory.ElasticCarriageReturnLineFeed.ToString();
    }

    // get accessibility string for use in code generation
    public static string GetAccessibilityString(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.Public => "public",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "public",
        };
    }
}
