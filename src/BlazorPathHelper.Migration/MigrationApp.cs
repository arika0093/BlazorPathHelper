using BlazorPathHelper.Migration.Builder;
using BlazorPathHelper.Migration.Factory;
using BlazorPathHelper.Migration.Helpers;
using BlazorPathHelper.Migration.Models;
using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using Sharprompt;
using ZLogger;

/// <summary>
/// This class contains the commands for the console application.
/// </summary>
internal class MigrationApp(
    ProjectSelectorHelper projectSelectHelper,
    PackageInstallHelper packageInstallHelper,
    SourceFileDataFactory sourceFileDataFactory,
    WebPathItemFactory webPathItemFactory,
    WebPathsFileExporter webPathsFileBuilder,
    CheckGitStatusHelper gitStatusHelper,
    ILogger<MigrationApp> logger
)
{
    /// <summary>
    /// Analyzes Razor components/pages in the specified project and generates code usable with BlazorPathHelper.
    /// </summary>
    /// <param name="projects">-p, The directory containing the .csproj file or the path to the `.csproj` file. If not specified, a selection form will be displayed.</param>
    /// <param name="replacePageAttrString">-r, Whether to replace the PageAttribute string. If not specified, confirmation will be prompted.</param>
    /// <param name="outputClassName">-c, The name of the generated class.</param>
    /// <param name="outputPath">-o, Specifies the output destination for the generated code.</param>
    /// <param name="forceExport">--force, Whether to overwrite the file if it already exists in the output destination. If not specified, confirmation will be prompted.</param>
    /// <param name="dryRun">Whether to perform a dry run. If true, the code will not be generated.</param>
    [Command("")]
    public async Task<int> Migration(
        string[]? projects = null,
        bool? replacePageAttrString = null,
        string outputClassName = "WebPaths",
        string outputPath = "./",
        bool forceExport = false,
        bool dryRun = false
    )
    {
        // parse command line arguments
        var convertedProjects = projectSelectHelper.ConvertProjectFileToDirectory(projects);
        var selectedProjects = convertedProjects.Length > 0
            ? convertedProjects
            : projectSelectHelper.SelectProjectsFromCurrentDirs();
        var isReplacePageAttributeString = replacePageAttrString ??
            Prompt.Confirm("Replace @page attribute string to generated variable ?", true);
        //var isQueryBuilderSupport = queryBuilderSupport ??
        //    Prompt.Confirm("Generate [Query] attributes from [SupportQueryBuilder] values ?", false);
        foreach (var project in selectedProjects)
        {
            var args = new CommandLineParsedArguments()
            {
                ProjectPath = project,
                OutputClassName = outputClassName,
                OutputDir = outputPath,
                ForceExport = forceExport,
                DisableInteractiveMode = false,
                IsDryRun = dryRun,
                IsReplacePageAttributeString = isReplacePageAttributeString,
                QueryBuilderSupport = false, // isQueryBuilderSupport
            };

            if(!gitStatusHelper.IsGitStatusClean(project) &&
                !Prompt.Confirm($"Git status is not clean. Do you want to continue ?", false))
            {
                logger.ZLogInformation($"Aborting migration.");
                return 1; // error
            }

            // try install
            await packageInstallHelper.InstallBlazorPathHelperToProject(project);
            // parse source files
            var sources = sourceFileDataFactory.FindSources(args.ProjectPath);
            var webPathItems = sources
                .SelectMany(source => webPathItemFactory.GenerateWebPathItem(source, args))
                .OrderBy(w => w.Path);
            // generate code
            var webPathsFile = webPathsFileBuilder.ExtendFileContent(webPathItems, args);
            // export code
            webPathsFileBuilder.ExportGeneratedWebPathsFile(webPathsFile, args);
            // replace @page attribute string
            webPathsFileBuilder.ReplaceRazorPageAttributeToVariable(webPathItems, args);
        }
        return 0; // success
    }

    /// <summary>
    /// print version information.
    /// </summary>
    public void Version()
    {
        Console.WriteLine(ThisAssembly.AssemblyInformationalVersion);
    }
}
