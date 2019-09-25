using Microsoft.Extensions.Logging;
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
    }
}
