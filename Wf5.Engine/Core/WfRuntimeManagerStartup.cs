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
    /// 流程启动运行时
    /// </summary>
    internal class WfRuntimeManagerStartup : WfRuntimeManager
    {
        internal override void ExecuteInstanceImp(IDbSession session)
        {
            //构造流程实例
            var processInstance = new ProcessInstanceManager()
                .CreateNewProcessInstanceObject(base.AppRunner, base.ProcessModel.ProcessEntity, 
                base.ParentProcessInstance, base.InvokedSubProcessNode == null? null : base.InvokedSubProcessNode.ActivityInstance);

            //构造活动实例
            //1. 获取开始节点活动
            var startActivity = base.ProcessModel.GetStartActivity();

            var startExecutionContext = ActivityForwardContext.CreateStartupContext(base.ProcessModel,
                processInstance,
                startActivity,
                base.ActivityResource);

            //base.RunWorkItemIteraly(startExecutionContext, session);
            NodeMediator mediator = NodeMediatorFactory.CreateNodeMediator(startExecutionContext, session);
            mediator.Linker.FromActivityInstance = RunningActivityInstance;
            mediator.ExecuteWorkItem();

            //构造回调函数需要的数据
            WfStartedResult result = base.WfExecutedResult as WfStartedResult;
            result.ProcessInstanceID = processInstance.ProcessInstanceID;
            result.Status = WfExecutedStatus.Success;
        }

        //private IList<NodeView> GetNextActivityTree(IDictionary<string, string> conditions, IDbSession session)
        //{
        //    //根据流程启动人信息，获取默认第一个节点办理任务
        //    var tm = new TaskManager();
        //    return tm.GetNextActivityTree(base.AppRunner, conditions, session);
        //}

        internal override RuntimeManagerType GetRuntimeManagerType()
        {
            return RuntimeManagerType.StartupRuntime;
        }
    }
}
