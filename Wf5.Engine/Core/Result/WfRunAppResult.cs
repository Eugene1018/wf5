using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wf5.Engine.Core.Result
{
    /// <summary>
    /// 流程运行结果
    /// </summary>
    public class WfRunAppResult : WfExecutedResult
    {
        /// <summary>
        /// 异常
        /// </summary>
        public WfRunAppException Exception { get; set; }
    }

    /// <summary>
    /// 运行流程异常
    /// </summary>
    public enum WfRunAppException
    {
        /// <summary>
        /// 错误参数
        /// </summary>
        ErrorArguments = 1,

        /// <summary>
        /// 没有任务
        /// </summary>
        HasNoTask = 2,

        /// <summary>
        /// 有超过1个以上的任务
        /// </summary>
        OverTasks = 3,

        /// <summary>
        /// 运行时错误
        /// </summary>
        RuntimeError = 4
    }
}
