using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkItemSync
{
    public class RequestRouter
    {
        private readonly ILogger _log;
        public RequestRouter(ILogger log)
        {
            _log = log;

        }

        internal void Route(dynamic workItemRequest)
        {
            throw new NotImplementedException();
        }
    }
}
