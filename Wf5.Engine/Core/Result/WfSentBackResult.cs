using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Core.Result
{         
    public class WfSentBackResult : WfExecutedResult
    {
        public WfSentBackException Exception { get; set; }

        public WfBackwardTaskReciever BackwardTaskReciever { get; set; }
    }

    public enum WfSentBackException
    {
        /// <summary>
        /// 不是任务节点
        /// </summary>
        NotTaskNode = 1,

        /// <summary>
        /// 是自身循环节点
        /// </summary>
        IsLoopNode = 2,

        /// <summary>
        /// 不在运行状态
        /// </summary>
        NotInRunning = 3,

        /// <summary>
        /// 不是登录用户的任务
        /// </summary>
        NotMineTask = 4,

        /// <summary>
        /// 前置步骤是开始节点
        /// </summary>
        IsStartNodePrevious = 5
    }
}
