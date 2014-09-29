using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Core.Result
{
    /// <summary>
    /// 撤销执行结果
    /// </summary>
    public class WfWithdrawResult : WfExecutedResult
    {
        public WfWithdrawException Exception { get; set; }

        public WfBackwardTaskReciever BackwardTaskReciever { get; set; }
    }

    /// <summary>
    /// 流程撤销异常状态
    /// </summary>
    public enum WfWithdrawException
    {
        /// <summary>
        /// 要撤销的节点不在待办状态
        /// </summary>
        NotInReady = 1,

        /// <summary>
        /// 不是登录用户创建的任务
        /// </summary>
        NotCreatedByMine = 2,

        /// <summary>
        /// 要撤销回去的节点有多个，允许只有一个
        /// </summary>
        WithdrawToHasTooMany = 3,

        /// <summary>
        /// 前置步骤是结束节点
        /// </summary>
        IsEndNodePrevious = 4
    }
}
