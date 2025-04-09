using System;
using System.Collections.Generic;
using System.Linq;
using BlazorPathHelper.Utils;
using Microsoft.CodeAnalysis;

namespace BlazorPathHelper.Models;

internal static class ParseRecordFactory
{
    /// <summary>
    /// create instance from BlazorPathAttribute.
    /// </summary>
    /// <param name="rootSymbol">symbol of class with BlazorPathAttribute</param>
    public static List<ParseRecord> GenerateRecordsFromPathAttr(INamedTypeSymbol rootSymbol)
    {
        return rootSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            // extract "const string" field with BlazorPathItemAttribute
            .Where(f => f.IsConst && f.Type.SpecialType == SpecialType.System_String)
            // and create instance from BlazorPathItemAttribute
            .Select(f => GenerateRecordFromPathAttr(rootSymbol, f))
            .ToList();
    }

    /// <summary>
    /// create instance from BlazorPathAttribute and BlazorPathItemAttribute.
    /// </summary>
    /// <param name="rootSymbol">symbol of class with BlazorPathAttribute</param>
    /// <param name="pathItemSymbol">symbol of `const string`</param>
    private static ParseRecord GenerateRecordFromPathAttr(
        INamedTypeSymbol rootSymbol,
        IFieldSymbol pathItemSymbol
    )
    {
        // get member value of BlazorPathAttribute
        var rootAttrDict = rootSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == nameof(BlazorPathAttribute))
            ?.ToDictionary();
        // This sequence is only called for classes with the BlazorPathAttribute attribute,
        // so it should be guaranteed to have the attribute.
        if (rootAttrDict == null)
        {
            throw new InvalidOperationException("BlazorPathAttribute is not found.");
        }

        var rootNamespace = rootAttrDict.Get(nameof(BlazorPathAttribute.Namespace));
        var rootClassName = rootAttrDict.Get(nameof(BlazorPathAttribute.ClassName));
        var rootPathBaseValue = rootAttrDict.Get(nameof(BlazorPathAttribute.PathBaseValue)) ?? "";
        var rootFileName = rootSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace("<", "_")
            .Replace(">", "_");

        // get member value of BlazorPathItemAttribute
        var pathItemAttr = pathItemSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == nameof(ItemAttribute));
        var pathItemDict = pathItemAttr?.ToDictionary();
        var pathRawValue = pathItemSymbol.ConstantValue?.ToString() ?? "";
        var itemVisible = pathItemDict?.Get(nameof(ItemAttribute.Visible));
        var itemIgnore = pathItemDict?.Get(nameof(ItemAttribute.Ignore));
        var itemNameFromProp = pathItemDict?.Get(nameof(ItemAttribute.Name));
        var itemNameFromConstructor = pathItemAttr?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        var itemDescription = pathItemDict?.Get(nameof(ItemAttribute.Description));
        var itemGroup = pathItemDict?.Get(nameof(ItemAttribute.Group));

        // check visibility
        bool? visibleFlag = itemVisible != null
            ? string.Compare(itemVisible, "true", StringComparison.OrdinalIgnoreCase) == 0
            : null;
        // check ignore flag
        bool? ignoreFlag = itemIgnore != null
            ? string.Compare(itemIgnore, "true", StringComparison.OrdinalIgnoreCase) == 0
            : null;

        // parse arguments of url
        var parseParameters = ParseParameterRecordFactory.CreateFromPath(pathRawValue);

        // get Blazor Page Type
        // PageAttribute<Page> and Page
        ExtractPageTypeSymbol(pathItemSymbol, out var blazorPageTypeSymbol, out var pageAttributeLocation);

        // icon is specified by generic or string. 
        // BlazorPathItemAttribute<Icon> -> new Icon()
        // BlazorPathItemAttribute(Icon = typeof(Icon)) -> new Icon()
        // BlazorPathItemAttribute(Icon = "icon-home") -> "icon-home"
        ExtractItemIconData(pathItemAttr, out var itemIcon, out var iconTypeSymbol);

        // parse query type
        ExtractQueryTypeSymbol(pathItemSymbol, out var queryTypeSymbol);

        List<ParseQueryRecord> queryRecords = [];
        if (queryTypeSymbol != null)
        {
            queryRecords = ParseQueryRecordFactory.CreateFromType(queryTypeSymbol);
        }

        return new()
        {
            BaseFileName = rootFileName,
            Namespace = rootNamespace ?? RoslynGeneratorUtilities.GetNamespace(rootSymbol.ContainingNamespace),
            AccessModifier = rootSymbol.DeclaredAccessibility.GetAccessibilityString(),
            ExportClassName = rootClassName ?? rootSymbol.Name,
            VariableName = pathItemSymbol.Name,
            PathBaseValue = rootPathBaseValue,
            PathRawValue = (string?)pathItemSymbol.ConstantValue ?? string.Empty,
            DisplayName = itemNameFromConstructor ?? itemNameFromProp ?? pathItemSymbol.Name,
            DisplayDescription = itemDescription,
            Parameters = parseParameters.ToList(),
            GroupPath = itemGroup ?? null,
            ForceDisplayFlag = visibleFlag,
            IsIgnore = ignoreFlag ?? false,
            Icon = itemIcon,
            IconSymbol = iconTypeSymbol,
            QueryTypeSymbol = queryTypeSymbol,
            QueryRecords = queryRecords,
            PageTypeSymbol = blazorPageTypeSymbol,
            PageAttributeLocation = pageAttributeLocation
        };
    }

    /// <summary>
    /// extract icon data from AttributeData of BlazorPathItemAttribute.
    /// </summary>
    private static void ExtractItemIconData(AttributeData? pathItemAttr,
        out string itemIcon, out ITypeSymbol? iconTypeSymbol)
    {
        itemIcon = "null";
        iconTypeSymbol = null;
        // PathItem(Icon = typeof(Icon)) -> Icon
        // PathItem(Icon = "icon-home") -> "icon-home"
        if (pathItemAttr == null)
        {
            return;
        }
        var symbol = pathItemAttr?.GetSymbol(nameof(ItemAttribute.Icon));
        if (symbol is ITypeSymbol iconSymbol)
        {
            iconTypeSymbol = iconSymbol;
        }
        else if (symbol is string iconText)
        {
            itemIcon = $"""" """{iconText}""" """";
        }
        // PathItem<Icon> -> new Icon()
        else if (pathItemAttr?.AttributeClass?.IsGenericType == true)
        {
            iconTypeSymbol = pathItemAttr.AttributeClass.TypeArguments.First();
        }
        // if icon is specified, generate code.
        if (iconTypeSymbol != null)
        {
            itemIcon = iconTypeSymbol.SpecialType == SpecialType.System_String
                ? $"""" """{iconTypeSymbol.Name}""" """"
                : $"new {iconTypeSymbol}()";
        }
    }

    /// <summary>
    /// extract query type symbol from AttributeData of BlazorPathItemAttribute.
    /// </summary>
    private static void ExtractQueryTypeSymbol(IFieldSymbol pathItemSymbol, out ITypeSymbol? queryTypeSymbol)
    {
        queryTypeSymbol = null;
        var pathQueryAttr = pathItemSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "QueryAttribute");
        if (pathQueryAttr is { AttributeClass.IsGenericType: true })
        {
            queryTypeSymbol = pathQueryAttr.AttributeClass.TypeArguments[0]; // TQuery
        }
    }

    /// <summary>
    /// extract page type symbol from AttributeData of BlazorPathItemAttribute.
    /// </summary>
    private static void ExtractPageTypeSymbol(IFieldSymbol pathItemSymbol,
        out ITypeSymbol? blazorPageTypeSymbol, out Location? pageAttributeLocation)
    {
        blazorPageTypeSymbol = null;
        pageAttributeLocation = null;
        var pathPageAttr = pathItemSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "PageAttribute");
        if (pathPageAttr is { AttributeClass.IsGenericType: true })
        {
           blazorPageTypeSymbol = pathPageAttr.AttributeClass.TypeArguments[0]; // TPage
        }

        var reference = pathPageAttr?.ApplicationSyntaxReference;
        if (reference == null) return;
        // get location of PageAttribute
        var span = reference.Span;
        var syntaxTree = reference.SyntaxTree;
        pageAttributeLocation = syntaxTree.GetLocation(span);
    }
}