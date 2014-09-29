using System;
using System.Threading;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Core.Result;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Core.Event;
using Wf5.Engine.Core.Pattern;


namespace Wf5.Engine.Core
{
    /// <summary>
    /// 退回流程运行时
    /// </summary>
    internal class WfRuntimeManagerSendBack : WfRuntimeManager
    {
        internal override void ExecuteInstanceImp(IDbSession session)
        {
            //创建新任务节点
            var backMostPreviouslyActivityInstanceID = GetBackwardMostPreviouslyActivityInstanceID();
            var nodeMediatorBackward = new NodeMediatorBackward(base.BackwardContext, session);
            nodeMediatorBackward.CreateBackwardActivityTaskTransitionInstance(base.BackwardContext.ProcessInstance,
                base.BackwardContext.BackwardFromActivityInstance,
                BackwardTypeEnum.Sendback,
                backMostPreviouslyActivityInstanceID,
                base.BackwardContext.BackwardToTargetTransitionGUID,
                TransitionTypeEnum.Sendback,
                TransitionFlyingTypeEnum.NotFlying,
                base.ActivityResource,
                session);

            //更新当前办理节点的状态（从准备或运行状态更新为退回状态）
            var aim = new ActivityInstanceManager();
            aim.SendBack(base.BackwardContext.BackwardFromActivityInstance.ActivityInstanceID, base.ActivityResource.AppRunner, session);

            //构造回调函数需要的数据
            WfSentBackResult result = base.WfExecutedResult as WfSentBackResult;
            result.BackwardTaskReciever = base.BackwardContext.BackwardTaskReciever;
            result.Status = WfExecutedStatus.Success;
        }

        internal override RuntimeManagerType GetRuntimeManagerType()
        {
            return RuntimeManagerType.SendBackRuntime;
        }
    }
}
