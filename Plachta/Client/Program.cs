using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Blazor.FileReader;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.DependencyInjection;
using Blazored.Modal;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Plachta.Client
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

            var host = builder.Build();

            host.Services
                .UseMaterialProviders()
                .UseMaterialIcons()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}
