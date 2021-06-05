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


            var webAssemblyHost = builder.Build();

            await ConfigureSignalR(webAssemblyHost.Services);

            await webAssemblyHost.RunAsync();
        }

        private static async Task ConfigureSignalR(IServiceProvider serviceProvider)
        {
            var signalRUrl = serviceProvider.GetSignalRUrl();
            Console.WriteLine($"Using SignalR URL: {signalRUrl}");

            var hubConnection = new HubConnectionBuilder()
                .WithUrl(signalRUrl)
                .Build();

            hubConnection.On<List<DashboardMessage>>("dashboardMessage", messages => { Console.WriteLine(string.Join(Environment.NewLine, messages.Select(message => message.Details))); });

            await hubConnection.StartAsync();
        }
    }
}