using System;
using System.Collections.Generic;
using System.Text;
using WorkItemSync.Models;

namespace WorkItemSync
{
    public interface ISyncEngine
    {
        bool CanHandler(WorkItemRequest request);
        void HandleRequest(WorkItemRequest request);
    }
}
