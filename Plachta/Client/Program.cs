using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Blazored.Modal;

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
                .AddBootstrapProviders()
                .AddFontAwesomeIcons()
                .AddBlazoredModal()
                .AddBlazoredLocalStorage();

            var host = builder.Build();

            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}
