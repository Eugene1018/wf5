using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Business;

namespace Wf5.Engine.Core.Result
{
    public class WfStartedResult : WfExecutedResult
    {
        public int ProcessInstanceID { get; set; }
        public WfStartedException Exception { get; set; }
    }

    public enum WfStartedException
    {
        IsRunningAlready = 1
    }
}
