using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WorkItemSync.Models;

namespace WorkItemSync
{
    public class RequestRouter
    {
        private readonly ILogger _log;
        public RequestRouter(ILogger log)
        {
            _log = log;
        }

        internal void Route(WorkItemRequest workItemRequest)
        {
            var devOpsConfig = DevOpsConfigFactory.GetConfiguration();
            if (devOpsConfig.SyncEnabledProjects.Contains(workItemRequest.TeamProject))
            {
                
                UserStorySync userStorySync = new UserStorySync(devOpsConfig, _log);
                if (userStorySync.CanHandler(workItemRequest))
                {
                    userStorySync.HandleRequest(workItemRequest);
                }

            }
            else
            {
                _log.LogInformation($"Skipping WorkItem for Project: {workItemRequest.TeamProject}");
            }
        }
    }
}
