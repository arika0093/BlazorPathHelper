using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using BlazorPathHelper.Migration.Helpers;
using BlazorPathHelper.Migration.Models;
using Microsoft.Extensions.Logging;
using Sharprompt;
using System.Text;
using System.Text.RegularExpressions;
using ZLogger;

namespace BlazorPathHelper.Migration.Builder;

/// <summary>
/// This class is responsible for exporting the generated web paths to a file.
/// </summary>
internal class WebPathsFileExporter(
    ILogger<WebPathsFileExporter> logger
)
{
    /// <summary>
    /// Extends the content of the generated file with additional web paths.
    /// </summary>
    public string ExtendFileContent(
        IEnumerable<WebPathItemStructure> webPaths,
        CommandLineParsedArguments args)
    {
        var content = "";
        var outputFile = args.OutputFileFullPath;
        if (Path.Exists(outputFile))
        {
            content = File.ReadAllText(outputFile);
        } else {
            content = PlainWebPathFileContent(args);
        }
        var syntaxTree = CSharpSyntaxTree.ParseText(content);
        var root = syntaxTree.GetRoot();

        // Find the class declaration in the syntax tree
        var classDeclaration = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        if (classDeclaration == null) {
            throw new InvalidOperationException("No class declaration found in the file.");
        }

        // Create a new field declaration for each web path
        var updatedClass = classDeclaration;
        foreach (var webpath in webPaths)
        {
            // if exist variable name, skip
            var variableName = webpath.NoConflictVariableName(webPaths);
            var existingField = classDeclaration.Members
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(f => f.Declaration.Variables
                    .Any(v => v.Identifier.Text == variableName));
            if (existingField != null)
            {
                logger.ZLogTrace($"Field {variableName} already exists. Skipping.");
                continue;
            }
            // Create a new field declaration
            // e.g. public const string Sample = "/sample";
            var newField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(variableName)
                        .WithInitializer(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(webpath.Path)))))))
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(webpath.Accessibility),
                    SyntaxFactory.Token(SyntaxKind.ConstKeyword)));
            // add
            updatedClass = updatedClass.AddMembers(newField);
        }
        // to string
        var updatedRoot = root.ReplaceNode(classDeclaration, updatedClass);
        var formattedCode = updatedRoot.NormalizeWhitespace().ToFullString();
        return formattedCode;
    }

    /// <summary>
    /// Generates the content of the web paths file in plain format.
    /// </summary>
    private string PlainWebPathFileContent(
        CommandLineParsedArguments args)
    {
        return $$"""
        #pragma warning disable CA1050

        using BlazorPathHelper;

        [BlazorPath]
        public partial class {{args.OutputClassName}}
        {
        }
        """;
    }

    /// <summary>
    /// Exports the generated file to the specified path.
    /// </summary>
    public void ExportGeneratedWebPathsFile(
        string contents,
        CommandLineParsedArguments args)
    {
        var outputPath = args.OutputFileFullPath;
        var outputFileName = Path.GetFileName(outputPath);
        if (File.Exists(outputPath) && !args.ForceExport)
        {
            var isOverwrite = Prompt.Confirm(
                $"File {outputFileName} already exists. Do you want to overwrite it?",
                false);
            if (!isOverwrite)
            {
                logger.LogInformation($"File {outputFileName} already exists. Skipping export.");
                return;
            }
        }
        if(args.IsDryRun)
        {
            logger.ZLogDebug($"Dry run: Exporting to {outputFileName}.");
            return;
        }
        File.WriteAllText(outputPath, contents);
        logger.LogInformation($"Exported to {outputFileName} successfully.");
    }

    /// <summary>
    /// Replaces the @page attribute in Razor files with a variable name.
    /// </summary>
    public void ReplaceRazorPageAttributeToVariable(
        IEnumerable<WebPathItemStructure> webPaths,
        CommandLineParsedArguments args
    )
    {
        if(!args.IsReplacePageAttributeString)
        {
            logger.ZLogInformation($"Skipping @page attribute replacement.");
            return;
        }
        foreach (var webPath in webPaths)
        {
            var filePath = webPath.Source.FilePath;
            var fileName = Path.GetFileName(filePath);
            var fileContent = webPath.Source.FileContent;
            var variableName = webPath.NoConflictVariableName(webPaths);
            var exportClass = args.OutputClassName;
            // replace @page\s+"{webPath.Path}" -> @attribute [Route({exportClass}.{variableName})]
            // use regex to replace
            var regex = new Regex($@"@page\s+""{webPath.Path}""", RegexOptions.IgnoreCase);
            var beforeAttr = $"@page \"{webPath.Path}\"";
            var afterAttr = $"@attribute [Route({exportClass}.{variableName})]";
            var newContent = regex.Replace(fileContent, afterAttr);
            // export to file
            if (args.IsDryRun)
            {
                logger.ZLogDebug($"Dry run: '{beforeAttr}' -> '{afterAttr}'");
                return;
            }
            else
            {
                File.WriteAllText(filePath, newContent);
                logger.ZLogInformation($"Replaced @page attribute in {fileName}.");
            }
        }
    }
}
