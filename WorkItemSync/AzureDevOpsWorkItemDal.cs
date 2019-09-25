using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WorkItemSync.Models;

namespace WorkItemSync
{
    public static class AzureDevOpsWorkItemDal
    {

        [Obsolete("Use GetWorkItemUsingClient")]
        public static async Task<string> GetWorkItem(DevOpsConfig config, int WorkItemId, ILogger log)
        {
            //https://docs.microsoft.com/en-us/rest/api/vsts/wit/work%20items/get%20work%20item?view=vsts-rest-4.1
            using (HttpClient client = new HttpClient())
            {
                SetAuthentication(client, config.UserName, config.PersonalAccessToken);

                var getUri = config.BaseUrl + "/" + config.MasterProjectName + "/_apis/wit/workitems/" + WorkItemId + "?$expand=all&api-version=4.1";
                log.LogInformation(getUri);

                using (HttpResponseMessage response = await client.GetAsync(
                           getUri))
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    log.LogInformation(responseBody);
                    response.EnsureSuccessStatusCode();
                    return responseBody;
                }
            }
        }

        [Obsolete("Use UpdateWorkItemUsingClient")]
        public static async Task UpdateWorkItem(DevOpsConfig config, int WorkItemId, string jsonPatch, ILogger log)
        {
            //https://docs.microsoft.com/en-us/rest/api/vsts/wit/work%20items/update?view=vsts-rest-4.1
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));
                SetAuthentication(client, config.UserName, config.PersonalAccessToken);

                var postUri = config.BaseUrl + "/" + config.MasterProjectName + "/_apis/wit/workitems/" + WorkItemId + "?api-version=4.1";
                log.LogInformation(postUri);

                using (HttpResponseMessage response = await client.PatchAsync(
                           postUri, new StringContent(jsonPatch, Encoding.UTF8, "application/json-patch+json")))
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    log.LogInformation(responseBody);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public static void SetAuthentication(HttpClient client, string userName, string PAT)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", userName, PAT))));
        }

        // https://github.com/microsoft/azure-devops-dotnet-samples
        public static async Task<WorkItem> GetWorkItemUsingClient(DevOpsConfig config, int WorkItemId, ILogger log)
        {
            Microsoft.VisualStudio.Services.WebApi.VssConnection connection = 
                new Microsoft.VisualStudio.Services.WebApi.VssConnection(
                    new Uri(config.BaseUrl), 
                    new Microsoft.VisualStudio.Services.Common.VssBasicCredential(string.Empty, config.PersonalAccessToken));

            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem workItem = await witClient.GetWorkItemAsync(WorkItemId);

            return workItem;
        }

        //https://github.com/microsoft/azure-devops-dotnet-samples/blob/d95b476b950301be4097a1dbd8e1c903faaac752/ClientLibrary/Samples/WorkItemTracking/WorkItemsSample.cs
        public static WorkItem UpdateWorkItemUsingClient(DevOpsConfig config, JsonPatchDocument patchDocument, int workItemId, ILogger log)
        {
            Microsoft.VisualStudio.Services.WebApi.VssConnection connection =
                new Microsoft.VisualStudio.Services.WebApi.VssConnection(
                    new Uri(config.BaseUrl),
                    new Microsoft.VisualStudio.Services.Common.VssBasicCredential(string.Empty, config.PersonalAccessToken));

            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem workItem = witClient.UpdateWorkItemAsync(patchDocument, workItemId).Result;

            return workItem;
        }
    }
}
