using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorPathHelper.Utils;
internal static class RoslynGeneratorUtilities
{
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
            _ => "public"
        };
    }
}
