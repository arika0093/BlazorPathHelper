// namespace BlazorPathHelper.Models;
//
// internal class ParseRecordTreeStructure
// {
//     public int Index { get; }
//     public PathAttributeParser Parser { get; }
//     public PathAttributeTreeParser[] ChildItems { get; }
//
//     public PathAttributeTreeParser(PathAttributeParser[] parsers, PathAttributeParser rootItem)
//     {
//         // 自分のパスが親のパスを含むものを抽出する。
//         // 末尾/は削除しておく。
//         var trimmedPath = rootItem.PathRawValue.TrimEnd('/');
//         var subMenu = parsers
//             .Where(p => p.GroupPath == trimmedPath && !p.IsRootMenuItem)
//             .Select(p => new PathAttributeTreeParser(parsers, p));
//         this.Index = Array.IndexOf(parsers, rootItem);
//         this.Parser = rootItem;
//         this.ChildItems = subMenu.ToArray();
//     }
//
//     public string ExportCode(int groupIndex, int groupLevel)
//     {
//         var code = $$"""
//                      new BlazorPathMenuItem(){ 
//                         Index = {{Index}},
//                         GroupKey = "{{Parser.GroupPath}}",
//                         GroupIndex = {{groupIndex}},
//                         GroupLevel = {{groupLevel}},
//                         Name = "{{Parser.DisplayName}}",
//                         Path = "{{Parser.PathRawValue}}",
//                         {{(Parser.DisplayDescription != "" ? $"Description = \"{Parser.DisplayDescription}\",\n" : "")}}
//                         {{(Parser.MenuIconBuilder != null ? $"Icon = {Parser.MenuIconBuilder},\n" : "")}}
//                         {{(ChildItems.Length > 0 ? $"Children = [{string.Join(",\n", ChildItems.Select((c,i) => c.ExportCode(i, groupLevel+1)))}]" : "")}}
//                      }
//                      """;
//         // trim linebreak and spaces
//         return code.Replace("\n", "").Replace("  ", "");
//     }
// }