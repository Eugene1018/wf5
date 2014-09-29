using System;
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
    internal class NodeMediatorXOrJoin : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorXOrJoin(ActivityEntity activity, ProcessModel processModel, IDbSession session)
            : base(activity, processModel, session)
        {

        }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            Guid transitionGUID,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource,
            IDbSession session)
        {
            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Unknown);

            bool canRenewInstance = false;

            //检查是否有运行中的合并节点实例
            ActivityInstanceEntity joinNode = base.ActivityInstanceManager.GetActivityWithRunningState(
                processInstance.ProcessInstanceID,
                base.GatewayActivity.ActivityGUID,
                session);

            if (joinNode == null)
            {
                canRenewInstance = true;
            }
            else
            {
                //判断是否可以激活下一步节点
                canRenewInstance = (joinNode.CanRenewInstance == 1);
                if (!canRenewInstance)
                {
                    result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.FallBehindOfXOrJoin);
                    return result;
                }
            }

            if (canRenewInstance)
            {
                var gatewayActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity, 
                    processInstance, activityResource.AppRunner);

                gatewayActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.XOrJoin;

                base.InsertActivityInstance(gatewayActivityInstance,
                    session);

                base.CompleteActivityInstance(gatewayActivityInstance.ActivityInstanceID,
                    activityResource,
                    session);

                gatewayActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                base.GatewayActivityInstance = gatewayActivityInstance;
                
                //写节点转移实例数据
                base.InsertTransitionInstance(processInstance,
                    transitionGUID,
                    fromActivityInstance,
                    gatewayActivityInstance,
                    TransitionTypeEnum.Forward,
                    TransitionFlyingTypeEnum.NotFlying,
                    activityResource.AppRunner,
                    session);

                result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            }
            return result;
        }
        #endregion
    }
}
