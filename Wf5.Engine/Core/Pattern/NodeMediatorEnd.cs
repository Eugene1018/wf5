using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Xpdl.Node;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;

namespace Wf5.Engine.Core.Pattern
{
    internal class NodeMediatorEnd : NodeMediator, ICompleteAutomaticlly
    {
        internal NodeMediatorEnd(ActivityForwardContext forwardContext, IDbSession session)
            : base(forwardContext, session)
        {
            
        }

        internal override void ExecuteWorkItem()
        {
            throw new NotImplementedException();
        }

        #region ICompleteAutomaticlly 成员
        /// <summary>
        /// 自动完成结束节点
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            Guid transitionGUID,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource,
            IDbSession session)
        {
            GatewayExecutedResult result = null;
            var toActivityInstance = base.CreateActivityInstanceObject(base.Linker.ToActivity, 
                processInstance, activityResource.AppRunner);

            base.ActivityInstanceManager.Insert(toActivityInstance, session);

            base.ActivityInstanceManager.Complete(toActivityInstance.ActivityInstanceID,
                activityResource.AppRunner,
                session);

            //写节点转移实例数据
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                TransitionTypeEnum.Forward,
                TransitionFlyingTypeEnum.NotFlying,
                activityResource.AppRunner,
                session);

            //设置流程完成
            ProcessInstanceManager pim = new ProcessInstanceManager();
            pim.Complete(processInstance.ProcessInstanceID, activityResource.AppRunner, session);

            //发送流程结束消息给流程启动人

            return result;
        }

        #endregion
    }
}
