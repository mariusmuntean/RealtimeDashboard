using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealtimeDashboard.Blazor.Extensions
{
    public static class IServiceProviderExtensions
    {
        public static string GetSignalRUrl(this IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var signalRUrl = config["Urls:SignalRUrl"];

            return signalRUrl;
        }
    }
}