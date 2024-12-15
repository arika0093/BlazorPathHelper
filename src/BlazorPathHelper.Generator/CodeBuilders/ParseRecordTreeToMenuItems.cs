using System.Linq;
using System.Text;
using BlazorPathHelper.Models;
using BlazorPathHelper.Utils;

namespace BlazorPathHelper.CodeBuilders;

internal class ParseRecordTreeToMenuItems(ParseRecordTreeStructure ts)
{
    private const int SpacesPerLevel = 8; // 2 * 4

    /// <summary>
    /// export code for menu generation.
    /// </summary>
    /// <param name="groupIndex">index in group</param>
    /// <param name="groupLevel">level in group</param>
    /// <returns>code for menu generation</returns>
    public string ExportMenuCode(int groupIndex, int groupLevel)
    {
        var nl = RoslynGeneratorUtilities.GetNewLine();
        var tab = new string(' ', (groupLevel + 1) * SpacesPerLevel);
        var sb = new StringBuilder();
        sb.Append(
            $$"""
              {{tab}}new BlazorPathMenuItem(){ 
              {{tab}}    Index = {{ts.Index}},
              {{tab}}    GroupKey = "{{ts.Record.GroupPath}}",
              {{tab}}    GroupIndex = {{groupIndex}},
              {{tab}}    GroupLevel = {{groupLevel}},
              {{tab}}    Name = "{{ts.Record.DisplayName}}",
              {{tab}}    Path = "{{ts.Record.PathRawValue}}",
              """);

        // export description
        if (!string.IsNullOrEmpty(ts.Record.DisplayDescription))
        {
            sb.Append(
                $"{nl}{tab}    Description = \"{ts.Record.DisplayDescription}\",");
        }
        // export icon
        if (ts.Record.Icon != null)
        {
            sb.Append(
                $"{nl}{tab}    Icon = {ts.Record.Icon},");
        }
        // export children
        var childMenuItems = ts.ChildItems.Select((c, i) =>
        {
            var builder = new ParseRecordTreeToMenuItems(c);
            return builder.ExportMenuCode(i, groupLevel + 1);
        }).ToList();
        if (childMenuItems.Any())
        {
            sb.Append(
                $"""

                 {tab}    Children = [
                 {string.Join($",{nl}", childMenuItems)}
                 {tab}    ]
                 """);
        }
        sb.Append($"{nl}{tab}}}");

        return sb.ToString();
    }
}