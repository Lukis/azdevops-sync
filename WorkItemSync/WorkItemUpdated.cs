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
    public static class WorkItemUpdateded
    {
        [FunctionName("WorkItemUpdated")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP triggered function WorkItemUpdated.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);

            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return (ActionResult)new OkObjectResult($"You Sent, {requestBody}");
        }
    }
}
