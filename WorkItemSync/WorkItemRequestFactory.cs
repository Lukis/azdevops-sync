using System;
using System.Collections.Generic;
using System.Text;
using WorkItemSync.Models;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace WorkItemSync
{
    public class WorkItemRequestFactory
    {
        public static WorkItemRequest GetRequest(dynamic data, string eventName)
        {
            var request = new WorkItemRequest();

            if (eventName.Equals("create"))
            {
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
            }
            else if (eventName.Equals("update"))
            {
                request.WorkItemType = Uri.UnescapeDataString((string)data["resource"]["revision"]["fields"]["System.WorkItemType"]);
                request.Id = (int)data["resource"]["revision"]["id"];
                request.Title = Uri.UnescapeDataString((string)data["resource"]["revision"]["fields"]["System.Title"]);
                request.TeamProject = Uri.UnescapeDataString((string)data["resource"]["revision"]["fields"]["System.TeamProject"]);
                //request.Tags = Uri.UnescapeDataString((string)data["resource"]["revision"]["fields"]["System.Tags"]).Split(',').ToList();
                request.EventName = eventName;
                request.State = Uri.UnescapeDataString((string)data["resource"]["revision"]["fields"]["System.State"]);
                request.StateReason = Uri.UnescapeDataString((string)data["resource"]["revision"]["fields"]["System.Reason"]);

                var relations = data["resource"]["revision"]["relations"];
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
            }
            else if (eventName.Equals("get"))
            {
                request.WorkItemType = Uri.UnescapeDataString((string)data["fields"]["System.WorkItemType"]);
                request.Id = (int)data["id"];
                request.Title = Uri.UnescapeDataString((string)data["fields"]["System.Title"]);
                request.TeamProject = Uri.UnescapeDataString((string)data["fields"]["System.TeamProject"]);
                request.EventName = eventName;
                request.State = Uri.UnescapeDataString((string)data["fields"]["System.State"]);
                request.StateReason = Uri.UnescapeDataString((string)data["fields"]["System.Reason"]);
                request.Rev = (string)data["rev"];

                var relations = data["relations"];
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
            }
            return request;
        }

        public static WorkItemRequest GetRequest(WorkItem workItem, string eventName)
        {
            var request = new WorkItemRequest();
            request.EventName = eventName;
            request.Id = workItem.Id.Value;
            request.Rev = workItem.Rev.Value.ToString();
            request.State = workItem.Fields["System.State"].ToString();
            request.StateReason = workItem.Fields["System.Reason"].ToString();

            request.WorkItemType = workItem.Fields["System.WorkItemType"].ToString();
            request.Title = workItem.Fields["System.Title"].ToString();
            request.TeamProject = workItem.Fields["System.TeamProject"].ToString();

            return request;
        }
    }
}
