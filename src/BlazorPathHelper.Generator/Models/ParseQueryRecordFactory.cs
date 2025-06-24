using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorPathHelper.Models;

// factory for ParseQueryRecord.
internal class ParseQueryRecordFactory
{
    // extract information from type.
    public static List<ParseQueryRecord> CreateFromType(ITypeSymbol symbol)
    {
        // get members of property or field.
        var membersOfProperty = symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(m =>
                m.IsDefinition
                && m is { IsReadOnly: false, DeclaredAccessibility: Accessibility.Public }
            )
            .ToList();
        var membersOfField = symbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(m =>
                m.IsDefinition
                && m is { IsReadOnly: false, DeclaredAccessibility: Accessibility.Public }
            )
            .ToList();
        var members = membersOfProperty.OfType<ISymbol>().Concat(membersOfField).ToList();
        // create ParseQueryRecords.
        return members.Select(CreateFromSymbol).ToList();
    }

    // extract information from property/member.
    public static ParseQueryRecord CreateFromSymbol(ISymbol symbol)
    {
        var name = symbol.Name;
        var urlName = ExtractQueryUrlNameFromAttribute(symbol);

        // check has initializer
        EqualsValueClauseSyntax? clauseSyntax = null;
        symbol.DeclaringSyntaxReferences.Any(syntaxRef =>
        {
            var syntaxNode = syntaxRef.GetSyntax();
            // get initialize value
            if (syntaxNode is ParameterSyntax parameterSyntax)
            {
                clauseSyntax = parameterSyntax.Default;
                return clauseSyntax != null;
            }
            return false;
        });
        // check is nullable
        var isNullable = symbol switch
        {
            IFieldSymbol fieldSymbol => fieldSymbol.NullableAnnotation
                == NullableAnnotation.Annotated,
            IPropertySymbol propertySymbol => propertySymbol.NullableAnnotation
                == NullableAnnotation.Annotated,
            _ => throw new ArgumentException("symbol is not field or property."),
        };

        return new ParseQueryRecord()
        {
            UrlName = urlName,
            Type = symbol switch
            {
                IFieldSymbol fieldSymbol => fieldSymbol.Type,
                IPropertySymbol propertySymbol => propertySymbol.Type,
                _ => throw new ArgumentException("symbol is not field or property."),
            },
            Name = name,
            InitialValue = clauseSyntax,
            IsNullable = isNullable,
        };
    }

    private static string ExtractQueryUrlNameFromAttribute(ISymbol symbol)
    {
        string? shortName = null;
        // [SupplyParameterFromQuery(Name = "short")] -> short
        var supplyParameterAttr = symbol
            .GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "SupplyParameterFromQueryAttribute");
        // [QueryName("short")] -> short
        var queryNameAttr = symbol
            .GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "QueryNameAttribute");

        if (supplyParameterAttr != null)
        {
            shortName = supplyParameterAttr
                ?.NamedArguments.FirstOrDefault(pair => pair.Key == "Name")
                .Value.Value?.ToString();
        }
        else if (queryNameAttr != null)
        {
            shortName = queryNameAttr?.ConstructorArguments.FirstOrDefault().Value?.ToString();
        }
        // if shortName is null, use property name.
        var urlName = shortName ?? symbol.Name;
        return urlName;
    }
}
