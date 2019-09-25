using Microsoft.Extensions.Logging;
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
            var originalWorkItemString = AzureDevOpsWorkItemDal.GetWorkItem(_config, request.OriginalId, _log).Result;

            // compare status
            dynamic data = JsonConvert.DeserializeObject(originalWorkItemString);
            var originalWorkItem = WorkItemRequestFactory.GetRequest(data, "get");

            // update original status if it's different
            if (request.State != originalWorkItem.State ||
                request.StateReason != originalWorkItem.StateReason)
            {
                string jsonContent =
@"[
  {
    ""op"": ""test"",
    ""path"": ""/rev"",
    ""value"": " + originalWorkItem.Rev + @"
  },
  {
    ""op"": ""replace"",
    ""path"": ""/fields/System.State"",
    ""value"": """ + request.State + @"""
  },
  {
    ""op"": ""add"",
    ""path"": ""/fields/System.Reason"",
    ""value"": """ + request.StateReason + @"""
  }
]";
                AzureDevOpsWorkItemDal.UpdateWorkItem(_config, originalWorkItem.Id, jsonContent, _log);
            }
        }
    }
}
