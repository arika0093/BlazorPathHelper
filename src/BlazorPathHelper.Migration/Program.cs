using BlazorPathHelper.Migration.Builder;
using BlazorPathHelper.Migration.Factory;
using BlazorPathHelper.Migration.Helpers;
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;

var app = ConsoleApp.Create()
    .ConfigureLogging(config => {
        config.ClearProviders();
#if DEBUG
        config.SetMinimumLevel(LogLevel.Debug);
#else
        config.SetMinimumLevel(LogLevel.Information);
#endif
        config.AddZLoggerConsole(options => {
            options.IncludeScopes = true;
            options.UsePlainTextFormatter(formatter => {
                formatter.SetPrefixFormatter($"{0:local-timeonly} [{1:short}] ",
                    (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp, info.LogLevel));
            });
        });
    })
    .ConfigureServices(services => {
        services.AddSingleton<ProjectSelectorHelper>();
        services.AddSingleton<PackageInstallHelper>();
        services.AddSingleton<CheckGitStatusHelper>();
        services.AddSingleton<WebPathItemFactory>();
        services.AddSingleton<SourceFileDataFactory>();
        services.AddSingleton<WebPathsFileExporter>();
        services.AddLogging();
    });
app.Add<MigrationApp>();
app.Run(args);
