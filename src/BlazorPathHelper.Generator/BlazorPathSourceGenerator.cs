using System;
using System.Collections.Generic;
using System.Linq;
using BlazorPathHelper.CodeBuilders;
using BlazorPathHelper.Models;
using BlazorPathHelper.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#pragma warning disable CS1591

namespace BlazorPathHelper;

[Generator]
public class BlazorPathHelperSourceGenerator : IIncrementalGenerator
{
    // ReSharper disable once InconsistentNaming
    private static readonly string NL = RoslynGeneratorUtilities.GetNewLine();

    // Initialize is called at startup to configure the generator
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sourceRoot = context.SyntaxProvider.ForAttributeWithMetadataName(
            "BlazorPathHelper.BlazorPathAttribute",
            static (_, _) => true,
            static (context, _) => context);

        context.RegisterSourceOutput(sourceRoot, EmitPathRoot);
    }

    private static void EmitPathRoot(SourceProductionContext context, GeneratorAttributeSyntaxContext source)
    {
        var symbol = (INamedTypeSymbol)source.TargetSymbol;
        var parseRecords = ParseRecordFactory.GenerateRecordsFromPathAttr(symbol);
        var treeStructures = ParseRecordTreeStructureFactory.Create(parseRecords);
        ExportBuilderCode(context, parseRecords);
        ExportMenuStructureCode(context, treeStructures);
    }
    
    private static void ExportBuilderCode(SourceProductionContext context, List<ParseRecord> records)
    {
        var no = 0;
        var recordsGroupBy = records.GroupBy(r => new {r.ExportClassName, r.Namespace});
        foreach (var recordsOfCls in recordsGroupBy)
        {
            no++;
            var fr = recordsOfCls.First();
            var builderCodes = recordsOfCls.SelectMany(r => {
                var builder = new ParseRecordToPathHelper(r);
                return builder.BuildPathHelpers();
            });
            var code = $$"""
            // <auto-generated />
            #nullable enable
            #pragma warning disable CS8600
            #pragma warning disable CS8601
            #pragma warning disable CS8602
            #pragma warning disable CS8603
            #pragma warning disable CS8604
            using System;
            namespace {{fr.Namespace}};
            
            {{fr.AccessModifier}} partial class {{fr.ExportClassName}}
            {
                /// <summary>
                /// Helper class for path building
                /// </summary>
                public static partial class Helper
                {
                    {{string.Join($"{NL}        ", builderCodes)}}
                }
            }
            """;
            context.AddSource($"BPH_{fr.ExportClassName}_{no:D4}_Builder.g.cs", code);
        }
    }
    
    private static void ExportMenuStructureCode(SourceProductionContext context, List<ParseRecordTreeStructure> treeRecords)
    {
        var no = 0;
        var recordsGroupBy = treeRecords.GroupBy(r => new {r.Record.ExportClassName, r.Record.Namespace});
        foreach (var recordsOfCls in recordsGroupBy)
        {
            no++;
            var fr = recordsOfCls.First().Record;
            var menuCodes = recordsOfCls.Select((t, i) => {
                var builder = new ParseRecordTreeToMenuItems(t);
                return builder.ExportMenuCode(i, 0);
            });
            var code = $$"""
            // <auto-generated />
            #nullable enable
            using System;
            using System.Collections.Generic;
            using BlazorPathHelper;
            namespace {{fr.Namespace}};
            
            {{fr.AccessModifier}} partial class {{fr.ExportClassName}}
            {
                /// <summary>
                /// Generated menu item from path definition (flattened)
                /// </summary>
                public static BlazorPathMenuItem[] MenuItemFlatten => MenuItem.ToFlatten(i => i.Children);
                /// <summary>
                /// Generated menu item from path definition (tree structure)
                /// </summary>
                public static readonly BlazorPathMenuItem[] MenuItem = [
            {{string.Join($",{NL}", menuCodes)}}
                ];
            }
            """;
            context.AddSource($"BPH_{fr.ExportClassName}_{no:D4}_Menu.g.cs", code); 
        }
    }

}


