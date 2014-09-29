using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine;

namespace Wf5.Engine.Xpdl
{
    /// <summary>
    /// 活动上的资源类
    /// </summary>
    public class ActivityResource
    {
        #region 属性、构造函数
        /// <summary>
        /// 当前流程执行用户
        /// </summary>
        public WfAppRunner AppRunner
        {
            get;
            set;
        }

        /// <summary>
        /// 带有执行人员信息的下一步节点列表
        /// </summary>
        public IDictionary<Guid, PerformerList> NextActivityPerformers
        {
            get;
            set;
        }

        public IDictionary<string, string> ConditionKeyValuePair
        {
            get;
            set;
        }

        public object[] UserParameters
        {
            get;
            set;
        }

        internal ActivityResource(WfAppRunner runner,
            IDictionary<Guid, PerformerList> nextActivityPerformers,
            IDictionary<string, string> conditionKeyValuePair = null)
        {
            AppRunner = runner;
            NextActivityPerformers = nextActivityPerformers;
            ConditionKeyValuePair = conditionKeyValuePair;
        }

        internal static IDictionary<Guid, PerformerList> CreateNextActivityPerformers(Guid activityGUID,
            int userID,
            string userName)
        {
            var performerList = new PerformerList();
            performerList.Add(new Performer(userID, userName));
            IDictionary<Guid, PerformerList> nextActivityPerformers = new Dictionary<Guid, PerformerList>();
            nextActivityPerformers.Add(activityGUID, performerList);

            return nextActivityPerformers;
        }
        #endregion
    }
}
