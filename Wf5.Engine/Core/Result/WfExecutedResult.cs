using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wf5.Engine.Core.Result
{
    /// <summary>
    /// 工作流执行结果对象
    /// </summary>
    public class WfExecutedResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 执行状态
        /// </summary>
        public  WfExecutedStatus Status { get; set; }

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public WfExecutedResult()
        {
            Status = WfExecutedStatus.Default;
        }

        /// <summary>
        /// 失败结果
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static WfExecutedResult Failed(string message = null)
        {
            var result = new WfExecutedResult();
            result.Status = WfExecutedStatus.Failed;
            result.Message = message;

            return result;
        }

        /// <summary>
        /// 成功结果
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static WfExecutedResult Success(string message = null)
        {
            var result = new WfExecutedResult();
            result.Status = WfExecutedStatus.Success;
            result.Message = message;
            return result;
        }
    }

    /// <summary>
    /// 工作流执行状态
    /// </summary>
    public enum WfExecutedStatus
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 2,

        /// <summary>
        /// 异常
        /// </summary>
        Exception = 3
    }
}
