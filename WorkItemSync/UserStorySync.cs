using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WorkItemSync.Models;

namespace WorkItemSync
{
    public class UserStorySync : ISyncEngine
    {
        private const string _ITEM_TYPE = "User Story";
        private DevOpsConfig _config;
        private ILogger _log;

        public UserStorySync(DevOpsConfig config, ILogger log)
        {
            _config = config;
            _log = log;
        }

        public bool CanHandler(WorkItemRequest request)
        {
            if (request.WorkItemType.Equals(_ITEM_TYPE, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public void HandleRequest(WorkItemRequest request)
        {
            // get original workItem
            var originalWI = AzureDevOpsWorkItemDal.GetWorkItemUsingClient(_config, request.OriginalId, _log).Result;
            var originalWIRequest = WorkItemRequestFactory.GetRequest(originalWI, "get");

            // compare status
            
            if (request.State != originalWIRequest.State ||
                request.StateReason != originalWIRequest.StateReason)
            {
                // update original status if it's different
                JsonPatchDocument patchDocument = new JsonPatchDocument();

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Test,
                        Path = "/rev",
                        Value = originalWIRequest.Rev
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.State",
                        Value = originalWIRequest.State
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Reason",
                        Value = originalWIRequest.StateReason
                    }
                );

                var workItemResult = AzureDevOpsWorkItemDal.UpdateWorkItemUsingClient(_config, patchDocument, originalWIRequest.Id, _log);

                //var originalWorkItemString = AzureDevOpsWorkItemDal.GetWorkItem(_config, request.OriginalId, _log).Result;
                //dynamic data = JsonConvert.DeserializeObject(originalWorkItemString);
                //var originalWorkItem = WorkItemRequestFactory.GetRequest(data, "get");
                //                string jsonContent =
                //@"[
                //  {
                //    ""op"": ""test"",
                //    ""path"": ""/rev"",
                //    ""value"": " + originalWorkItem.Rev + @"
                //  },
                //  {
                //    ""op"": ""replace"",
                //    ""path"": ""/fields/System.State"",
                //    ""value"": """ + request.State + @"""
                //  },
                //  {
                //    ""op"": ""add"",
                //    ""path"": ""/fields/System.Reason"",
                //    ""value"": """ + request.StateReason + @"""
                //  }
                //]";
                //                AzureDevOpsWorkItemDal.UpdateWorkItem(_config, originalWorkItem.Id, jsonContent, _log);
            }
        }
    }
}
