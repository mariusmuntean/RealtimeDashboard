using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RealtimeDashboard.API
{
    public static class BoardcastChange
    {
        [FunctionName("Negotiate")]
        public static SignalRConnectionInfo GetConnectionInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest httpRequest,
            [SignalRConnectionInfo(HubName = "Dashboard", ConnectionStringSetting = "SignalRConnString")]
            SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("BoardcastChange")]
        public static async Task RunAsync([CosmosDBTrigger(
                databaseName: "Weather",
                collectionName: "WeatherData",
                ConnectionStringSetting = "CosmosDbConnString",
                LeaseCollectionName = "leases")]
            IReadOnlyList<Document> input,
            ILogger log,
            [SignalR(HubName = "Dashboard")] IAsyncCollector<SignalRMessage> dashboardMessageCollector)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation(string.Join(Environment.NewLine, input.Select(document => JsonConvert.SerializeObject(document, Formatting.Indented))));

                var messages = input.Select(document => new DashboardMessage {Id = document.Id, Details = document.GetPropertyValue<string>("temperature")});

                await dashboardMessageCollector.AddAsync(new SignalRMessage {Target = "dashboardMessage", Arguments = new[] {messages.ToArray()}});
            }
        }
    }

    public class DashboardMessage
    {
        public string Id { get; set; }

        public string Details { get; set; }
    }
}