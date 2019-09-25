using System;
using System.Collections.Generic;
using System.Text;
using WorkItemSync.Models;
using Newtonsoft.Json;
using System.Linq;

namespace WorkItemSync
{
    public class WorkItemRequestFactory
    {
        public static WorkItemRequest GetRequest(dynamic data, string eventName)
        {
            var request = new WorkItemRequest();
            
            request.WorkItemType = Uri.UnescapeDataString((string)data["resource"]["fields"]["System.WorkItemType"]);
            request.Id = (int)data["resource"]["id"];
            request.Title = Uri.UnescapeDataString((string)data["resource"]["fields"]["System.Title"]);
            request.TeamProject = Uri.UnescapeDataString((string)data["resource"]["fields"]["System.TeamProject"]);
            request.Tags = Uri.UnescapeDataString((string)data["resource"]["fields"]["System.Tags"]).Split(',').ToList();
            request.EventName = eventName;
            request.State = Uri.UnescapeDataString((string)data["resource"]["fields"]["System.State"]);
            request.StateReason = Uri.UnescapeDataString((string)data["resource"]["fields"]["System.Reason"]);

            var relations = data["resource"]["relations"];
            foreach (var relation in relations)
            {
                var relType = Uri.UnescapeDataString((string)relation["rel"]);
                var relUrl = Uri.UnescapeDataString((string)relation["url"]);

                if (relType.Equals("System.LinkTypes.Related"))
                {
                    string originalId = relUrl.Substring(relUrl.LastIndexOf('/') + 1);
                    int tempId = 0;
                    if (int.TryParse(originalId, out tempId))
                    {
                        request.OriginalId = tempId;
                    }
                }
            }
            return request;
        }
    }
}
