using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WorkItemSync
{
    public static class WorkItemCreated
    {
        [FunctionName("WorkItemCreated")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP triggered function WorkItemCreated.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);

            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var workItemRequest = WorkItemRequestFactory.GetRequest(data, "create");

            RequestRouter router = new RequestRouter(log);
            router.Route(workItemRequest);

            return (ActionResult)new OkObjectResult($"You Sent, {requestBody}");
        }
    }
}
