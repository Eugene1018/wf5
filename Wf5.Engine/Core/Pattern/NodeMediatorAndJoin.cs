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
    internal class NodeMediatorAndJoin : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorAndJoin(ActivityEntity activity, ProcessModel processModel, IDbSession session)
            : base(activity, processModel, session)
        {

        }

        internal int GetTokensRequired()
        {
            int tokensRequired = this.ProcessModel.GetBackwardTransitionListCount(GatewayActivity.ActivityGUID);
            return tokensRequired;
        }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            Guid transitionGUID,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource,
            IDbSession session)
        {
            //检查是否有运行中的合并节点实例
            ActivityInstanceEntity joinNode = base.ActivityInstanceManager.GetActivityWithRunningState(
                processInstance.ProcessInstanceID,
                base.GatewayActivity.ActivityGUID,
                session);

            if (joinNode == null)
            {
                var joinActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity, 
                    processInstance, activityResource.AppRunner);

                //计算总需要的Token数目
                joinActivityInstance.TokensRequired = GetTokensRequired();
                joinActivityInstance.TokensHad = 1;

                //进入运行状态
                joinActivityInstance.ActivityState = (short)ActivityStateEnum.Running;
                joinActivityInstance.GatewayDirectionTypeID = (short)GatewayDirectionEnum.AndJoin;

                base.InsertActivityInstance(joinActivityInstance,
                    session);
            }
            else
            {
                //更新节点的活动实例属性
                base.GatewayActivityInstance = joinNode;
                int tokensRequired = base.GatewayActivityInstance.TokensRequired;
                int tokensHad = base.GatewayActivityInstance.TokensHad;

                //更新Token数目
                base.ActivityInstanceManager.IncreaseTokensHad(base.GatewayActivityInstance.ActivityInstanceID,
                    activityResource.AppRunner,
                    session);

                if ((tokensHad + 1) == tokensRequired)
                {
                    //如果达到完成节点的Token数，则设置该节点状态为完成
                    base.CompleteActivityInstance(base.GatewayActivityInstance.ActivityInstanceID,
                        activityResource,
                        session);
                    base.GatewayActivityInstance.ActivityState = (short)ActivityStateEnum.Completed;
                }
            }

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(
                GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
