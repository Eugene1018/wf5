﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Xpdl.Node;

namespace Wf5.Engine.Core.Pattern
{
    internal class NodeMediatorOrJoin : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorOrJoin(ActivityEntity activity, ProcessModel processModel, IDbSession session)
            : base(activity, processModel, session)
        {

        }

        #region ICompleteAutomaticlly 成员

        /// <summary>
        /// OrJoin合并时的节点完成方法
        /// 1. 如果有满足条件的OrJoin前驱转移，则会重新生成新的OrJoin节点实例
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="fromTransition"></param>
        /// <param name="fromActivityInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="wfLinqDataContext"></param>
        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            Guid transitionGUID,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource,
            IDbSession session)
        {
            var gatewayActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity, 
                processInstance, activityResource.AppRunner);

            gatewayActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.OrJoin;
            base.InsertActivityInstance(gatewayActivityInstance,
                session);

            base.CompleteActivityInstance(gatewayActivityInstance.ActivityInstanceID,
                activityResource,
                session);

            gatewayActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
            base.GatewayActivityInstance = gatewayActivityInstance;
                        
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                gatewayActivityInstance,
                TransitionTypeEnum.Forward,
                TransitionFlyingTypeEnum.NotFlying,
                activityResource.AppRunner,
                session);

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
