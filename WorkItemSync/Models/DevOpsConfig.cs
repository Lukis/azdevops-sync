using System;
using System.Collections.Generic;
using System.Text;

namespace WorkItemSync.Models
{
    public class DevOpsConfig
    {
        public string BaseUrl { get; set; }
        public List<string> SyncEnabledProjects { get; set; }
        public string PersonalAccessToken { get; set; }
        public string MasterProjectName { get; set; }
        public string UserName { get; internal set; }
    }
}
