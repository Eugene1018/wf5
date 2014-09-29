using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Core.Result
{
    /// <summary>
    /// 返签结果
    /// </summary>
    public class WfReversedResult : WfExecutedResult
    {
        public WfReversedException Exception { get; set; }

        public WfBackwardTaskReciever BackwardTaskReciever { get; set; }
    }

    /// <summary>
    /// 返签异常
    /// </summary>
    public enum WfReversedException
    {
        NotInCompleted = 1
    }
}
