using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WorkItemSync.Models;

namespace WorkItemSync
{
    static class DevOpsConfigFactory
    {
        public static DevOpsConfig GetConfiguration()
        {
            var config = new DevOpsConfig();
            config.BaseUrl = GetEnvironmentVariable("DevOpsOrganizationUrl");
            config.MasterProjectName = GetEnvironmentVariable("MasterProject");
            config.PersonalAccessToken = GetEnvironmentVariable("PersonalAccessToken");
            config.UserName = GetEnvironmentVariable("UserName"); ;
            string syncProjects = GetEnvironmentVariable("SyncEnabledProjects");
            config.SyncEnabledProjects = syncProjects.Split(';').ToList();

            return config;
        }

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
