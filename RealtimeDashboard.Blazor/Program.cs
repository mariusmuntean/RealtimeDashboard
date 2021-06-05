using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealtimeDashboard.Blazor.Extensions;
using RealtimeDashboard.Common;

namespace RealtimeDashboard.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            await AddSignalR(builder.Services);

            var webAssemblyHost = builder.Build();
            await webAssemblyHost.RunAsync();
        }

        private static async Task AddSignalR(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<HubConnection>(provider =>
            {
                var signalRUrl = provider.GetSignalRUrl();
                Console.WriteLine($"Using SignalR URL: {signalRUrl}");
                
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl(signalRUrl)
                    .Build();

                return hubConnection;
            });
        }
    }
}