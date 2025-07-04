using AntDesign;
using Example.AntBlazor.Pro;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Example.AntBlazor.Standard
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

            await builder.Build().RunAsync();
        }
    }
}
