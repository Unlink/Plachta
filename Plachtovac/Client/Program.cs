using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.FileReader;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Plachtovac.Client.Services;

namespace Plachtovac.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services
                .AddBlazorContextMenu()
                .AddBlazorise(options => { options.ChangeTextOnKeyPress = true; })
                .AddMaterialProviders()
                .AddMaterialIcons()
                .AddFontAwesomeIcons()
                .AddBlazoredModal()
                .AddBlazoredLocalStorage()
                .AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<PlachtaService>();

            var host = builder.Build();

            host.Services
                .UseMaterialProviders()
                .UseMaterialIcons()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}
