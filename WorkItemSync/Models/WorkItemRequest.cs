using System;
using System.Collections.Generic;
using System.Text;

namespace WorkItemSync.Models
{
    public class WorkItemRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TeamProject { get; set; }
        public string WorkItemType { get; set; }
        public string EventName { get; set; }
        public int OriginalId { get; set; }
        public List<string> Tags { get; set; }
        public string State { get; internal set; }
        public string StateReason { get; internal set; }
        public string Rev { get; internal set; }
    }
}
