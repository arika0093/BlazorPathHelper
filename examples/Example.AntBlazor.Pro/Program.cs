using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Example.AntBlazor.Pro
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
            });
            builder.Services.AddAntDesign();
            builder.Services.Configure<ProSettings>(
                builder.Configuration.GetSection("ProSettings")
            );

            await builder.Build().RunAsync();
        }
    }
}
