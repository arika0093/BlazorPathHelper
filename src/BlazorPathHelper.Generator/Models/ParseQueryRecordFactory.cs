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
        var membersOfProperty = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(m => m.IsDefinition && m is { IsReadOnly: false, DeclaredAccessibility: Accessibility.Public })
            .ToList();
        var membersOfField = symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(m => m.IsDefinition && m is { IsReadOnly: false, DeclaredAccessibility: Accessibility.Public })
            .ToList();
        var members = membersOfProperty.OfType<ISymbol>().Concat(membersOfField).ToList();
        // create ParseQueryRecords.
        return members.Select(CreateFromSymbol).ToList();
    }

    // extract information from property/member.
    public static ParseQueryRecord CreateFromSymbol(ISymbol symbol)
    {
        var name = symbol.Name; 
        // [SupplyParameterFromQuery(Name = "short")] -> short
        var supplyParameterAttr = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "SupplyParameterFromQueryAttribute");
        var shortName = supplyParameterAttr?.NamedArguments
            .FirstOrDefault(pair => pair.Key == "Name").Value.Value?.ToString();
        // if shortName is null, use property name.
        var urlName = shortName ?? name;

        // check has initializer
        var hasInitializer = symbol.DeclaringSyntaxReferences.Any(syntaxRef =>
        {
            var syntaxNode = syntaxRef.GetSyntax();
            if (syntaxNode is PropertyDeclarationSyntax propertyDeclaration)
            {
                return propertyDeclaration.Initializer != null;
            }
            return false;
        });
        // check is nullable
        var isNullable = symbol switch
        {
            IFieldSymbol fieldSymbol => fieldSymbol.NullableAnnotation == NullableAnnotation.Annotated,
            IPropertySymbol propertySymbol => propertySymbol.NullableAnnotation == NullableAnnotation.Annotated,
            _ => throw new ArgumentException("symbol is not field or property.")
        };

        return new ParseQueryRecord() {
            UrlName = urlName,
            Type = symbol switch
            {
                IFieldSymbol fieldSymbol => fieldSymbol.Type,
                IPropertySymbol propertySymbol => propertySymbol.Type,
                _ => throw new ArgumentException("symbol is not field or property.")
            },
            Name = name,
            HasInitializer = hasInitializer,
            IsNullable = isNullable
        };
    }
}