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
        var rootFileName = rootSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace("<", "_")
            .Replace(">", "_");

        // get member value of BlazorPathItemAttribute
        var pathItemAttr = pathItemSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == nameof(BlazorPathItemAttribute));
        var pathItemDict = pathItemAttr?.ToDictionary();
        var pathRawValue = pathItemSymbol.ConstantValue?.ToString() ?? "";
        var itemVisible = pathItemDict?.Get(nameof(BlazorPathItemAttribute.Visible));
        var itemNameFromProp = pathItemDict?.Get(nameof(BlazorPathItemAttribute.Name));
        var itemNameFromConstructor = pathItemAttr?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        var itemDescription = pathItemDict?.Get(nameof(BlazorPathItemAttribute.Description));
        var itemGroup = pathItemDict?.Get(nameof(BlazorPathItemAttribute.Group));

        // check visibility
        var isHiddenFlag = string.Compare(itemVisible ?? "", "false", StringComparison.OrdinalIgnoreCase) == 0;

        // parse arguments of url
        var parseParameters = ParseParameterRecordFactory.CreateFromPath(pathRawValue);

        // get Blazor Page Type
        ITypeSymbol? blazorPageTypeSymbol = null;
        var pageType = pathItemAttr?.GetSymbol(nameof(BlazorPathItemAttribute.Page));
        if (pageType is ITypeSymbol pageSymbol)
        {
            blazorPageTypeSymbol = pageSymbol;
        }

        // icon is specified by generic or string. 
        // BlazorPathItemAttribute<Icon> -> new Icon()
        // BlazorPathItemAttribute(Icon = typeof(Icon)) -> new Icon()
        // BlazorPathItemAttribute(Icon = "icon-home") -> "icon-home"
        ExtractItemIconData(pathItemAttr, out var itemIcon, out var iconTypeSymbol);

        // parse query type
        ExtractQueryTypeSymbol(pathItemSymbol, out var queryTypeSymbol);

        List<ParseQueryRecord> queryRecords = [];
        if(queryTypeSymbol != null)
        {
            queryRecords = ParseQueryRecordFactory.CreateFromType(queryTypeSymbol);
        }

        return new ()
        {
            BaseFileName = rootFileName,
            Namespace = rootNamespace ?? RoslynGeneratorUtilities.GetNamespace(rootSymbol.ContainingNamespace),
            AccessModifier = rootSymbol.DeclaredAccessibility.GetAccessibilityString(),
            ExportClassName = rootClassName ?? rootSymbol.Name,
            VariableName = pathItemSymbol.Name,
            PathRawValue = (string?)pathItemSymbol.ConstantValue ?? string.Empty,
            IsDisplay = !isHiddenFlag,
            DisplayName = itemNameFromConstructor ?? itemNameFromProp ?? pathItemSymbol.Name,
            DisplayDescription = itemDescription,
            Parameters = parseParameters.ToList(),
            GroupPath = itemGroup ?? null,
            Icon = itemIcon,
            IconSymbol = iconTypeSymbol,
            QueryTypeSymbol = queryTypeSymbol,
            QueryRecords = queryRecords,
            PageTypeSymbol = blazorPageTypeSymbol,
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
        if(pathItemAttr == null) {
            return;
        }
        var symbol = pathItemAttr?.GetSymbol(nameof(BlazorPathItemAttribute.Icon));
        if (symbol is ITypeSymbol iconSymbol)
        {
            iconTypeSymbol = iconSymbol;
        }
        else if (symbol is string iconText)
        {
            itemIcon = $"\"{iconText}\"";
        }
        // PathItem<Icon> -> new Icon()
        else if (pathItemAttr?.AttributeClass?.IsGenericType == true)
        {
            iconTypeSymbol = pathItemAttr.AttributeClass.TypeArguments.First();
        }
        // if icon is specified, generate code.
        if (iconTypeSymbol != null)
        {
            itemIcon = iconTypeSymbol.SpecialType == SpecialType.System_String ? $"\"{iconTypeSymbol.Name}\"" : $"new {iconTypeSymbol}()";
        }
    }

    /// <summary>
    /// extract query type symbol from AttributeData of BlazorPathItemAttribute.
    /// </summary>
    private static void ExtractQueryTypeSymbol(IFieldSymbol pathItemSymbol, out ITypeSymbol? queryTypeSymbol)
    {
        queryTypeSymbol = null;
        var pathQueryAttr = pathItemSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "BlazorPathQueryAttribute");
        if (pathQueryAttr is { AttributeClass.IsGenericType: true })
        {
            queryTypeSymbol = pathQueryAttr.AttributeClass.TypeArguments[0]; // TQuery
        }
    }
}