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
using Wf5.Engine.Core.Event;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Xpdl.Node;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Core.Pattern;

namespace Wf5.Engine.Core
{
    /// <summary>
    /// 流程运行时管理
    /// </summary>
    internal abstract class WfRuntimeManager
    {
        #region 抽象方法
        internal abstract void ExecuteInstanceImp(IDbSession session);
        internal abstract RuntimeManagerType GetRuntimeManagerType();
        #endregion

        #region 流转属性和基础方法
        internal WfAppRunner AppRunner { get; set; }
        internal ProcessModel ProcessModel { get; set; }
        internal ProcessInstanceEntity ParentProcessInstance { get; set; }
        internal NodeBase InvokedSubProcessNode { get; set; }
        internal ActivityResource ActivityResource { get; set; }
        internal TaskViewEntity TaskView { get; set; }
        internal ActivityInstanceEntity RunningActivityInstance { get; set; }
        
        //流程返签时的属性
        internal BackwardContext BackwardContext { get; set; }

        /// <summary>
        /// 流程执行结果对象
        /// </summary>
        internal WfExecutedResult WfExecutedResult { get; set; }

        /// <summary>
        /// 获取退回时最早节点实例ID，支持连续退回
        /// </summary>
        /// <returns></returns>
        protected int GetBackwardMostPreviouslyActivityInstanceID()
        {
            //获取退回节点实例ID
            int backMostPreviouslyActivityInstanceID;
            if (BackwardContext.BackwardToTaskActivityInstance.BackSrcActivityInstanceID != null)
                backMostPreviouslyActivityInstanceID = BackwardContext.BackwardToTaskActivityInstance.BackSrcActivityInstanceID.Value;
            else
                backMostPreviouslyActivityInstanceID = BackwardContext.BackwardToTaskActivityInstance.ActivityInstanceID;

            return backMostPreviouslyActivityInstanceID;
        }
        
        #endregion

        #region 构造方法
        internal WfRuntimeManager()
        {
            AppRunner = new WfAppRunner();
            BackwardContext = new BackwardContext();
        }
        #endregion

        #region WfRuntimeManager 创建执行实例的运行者对象
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="user"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="nextActivityGUID"></param>
        /// <returns></returns>
        public static WfRuntimeManager CreateRuntimeInstanceStartup(WfAppRunner runner,
            ref WfStartedResult result)
        {
            return CreateRuntimeInstanceStartup(runner, null, null, ref result);
        }

        public static WfRuntimeManager CreateRuntimeInstanceStartup(WfAppRunner runner,
            ProcessInstanceEntity parentProcessInstance,
            SubProcessNode subProcessNode,
            ref WfStartedResult result)
        {
            //检查流程是否可以被启动
            var rmins = new WfRuntimeManagerStartup();
            rmins.WfExecutedResult = result = new WfStartedResult();
            var pim = new ProcessInstanceManager();
            ProcessInstanceEntity processInstance = null;
            if (subProcessNode == null)
            {
                //子流程启动
                processInstance = pim.GetProcessInstanceLatest(runner.AppName,
                    runner.AppInstanceID,
                    runner.ProcessGUID);
            }
            else
            {
                //正常流程启动
                processInstance = pim.GetProcessInstanceLatest(runner.AppName,
                    runner.AppInstanceID,
                    subProcessNode.SubProcessGUID.Value);
            }

            //不能同时启动多个主流程
            if (processInstance != null 
                && processInstance.ParentProcessInstanceID == null
                && processInstance.ProcessState == (short)ProcessStateEnum.Running)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfStartedException.IsRunningAlready;
                result.Message = "当前应用已经有流程实例在运行中，除非终止或取消流程，否则流程不能被再次启动。";
                return rmins;
            }

            rmins.AppRunner = runner;
            rmins.ParentProcessInstance = parentProcessInstance;
            rmins.InvokedSubProcessNode = subProcessNode;

            //获取流程第一个可办理节点
            rmins.ProcessModel = new ProcessModel(runner.ProcessGUID);
            var firstActivity = rmins.ProcessModel.GetFirstActivity();

            rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(firstActivity.ActivityGUID,
                runner.UserID,
                runner.UserName);

            rmins.ActivityResource = new ActivityResource(runner, rmins.AppRunner.NextActivityPerformers);

            return rmins;
        }

        /// <summary>
        /// 根据办理的业务数据，获取流程信息
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="nextActivityPerformers"></param>
        /// <returns></returns>
        public static WfRuntimeManager CreateRuntimeInstanceAppRunning(WfAppRunner runner,
            ref WfRunAppResult result)
        {
            //检查传人参数是否有效
            var rmins = new WfRuntimeManagerAppRunning();
            rmins.WfExecutedResult = result = new WfRunAppResult();
            if (string.IsNullOrEmpty(runner.AppName) || runner.AppInstanceID == 0 || runner.ProcessGUID == null)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfRunAppException.ErrorArguments;
                result.Message = "方法参数错误，无法运行流程！";
                return rmins;
            }

            //传递runner变量
            rmins.AppRunner = runner;

            var aim = new ActivityInstanceManager();
            TaskViewEntity taskView = null;
            var runningNode = aim.GetRunningNode(runner, out taskView);

            if (runningNode.AssignedToUsers.Contains(runner.UserID.ToString()) == false)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfRunAppException.HasNoTask;
                result.Message = "当前没有登录用户要办理的任务，无法运行流程！";
                return rmins;
            }

            var processModel = new ProcessModel(runningNode.ProcessGUID);
            var activityResource = new ActivityResource(runner, runner.NextActivityPerformers, runner.Conditions);

            var tm = new TaskManager();
            rmins.TaskView = taskView;
            rmins.RunningActivityInstance = runningNode;
            rmins.ProcessModel = processModel;
            rmins.ActivityResource = activityResource;

            return rmins;
        }

        /// <summary>
        /// 撤销操作
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static WfRuntimeManager CreateRuntimeInstanceWithdraw(WfAppRunner runner,
            ref WfWithdrawResult result)
        {
            //获取当前运行节点信息
            var rmins = new WfRuntimeManagerWithdraw();
            rmins.WfExecutedResult = result = new WfWithdrawResult();
            var aim = new ActivityInstanceManager();
            var runningNode = aim.GetRunningNode(runner);
            if ((runningNode == null) || (runningNode.ActivityState != (short)ActivityStateEnum.Ready))
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfWithdrawException.NotInReady;
                result.Message = string.Format("要撤销的节点不在准备状态，已经无法撤销到上一步，节点状态：{0}",
                   runningNode.ActivityState);
            }

            //获取上一步流转节点信息，可能经过And, Or等路由节点
            var tim = new TransitionInstanceManager();
            bool hasGatewayNode = false;
            var lastActivityInstanceList = tim.GetPreviousActivityInstance(runningNode, false, out hasGatewayNode).ToList();

            if (lastActivityInstanceList == null || lastActivityInstanceList.Count > 1)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfWithdrawException.WithdrawToHasTooMany;
                result.Message = "当前没有可以撤销回去的节点，或者有多个可以撤销回去的节点，无法选择！";

                return rmins;
            }

            TransitionInstanceEntity lastTaskTransitionInstance = null;
            if (hasGatewayNode == false)
            {
                lastTaskTransitionInstance = tim.GetLastTaskTransition(runner.AppName, runner.AppInstanceID, runner.ProcessGUID);
            }

            var withdrawActivityInstance = lastActivityInstanceList[0];
            if (withdrawActivityInstance.EndedByUserID.Value != runner.UserID)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfWithdrawException.NotCreatedByMine;
                result.Message = string.Format("上一步节点的任务办理人跟当前登录用户不一致，无法撤销回上一步！节点办理人：{0}",
                    withdrawActivityInstance.EndedByUserName);

                return rmins;
            }

            if (withdrawActivityInstance.ActivityType == (short)ActivityTypeEnum.EndNode)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfWithdrawException.IsEndNodePrevious;
                result.Message = "上一步是结束节点，无法撤销！";

                return rmins;
            }

            //准备撤销节点的相关信息
            var processModel = (new ProcessModel(runner.ProcessGUID));
            rmins.ProcessModel = processModel;
            rmins.AppRunner.ProcessGUID = runner.ProcessGUID;
            rmins.BackwardContext.ProcessInstance = (new ProcessInstanceManager()).GetById(runningNode.ProcessInstanceID);
            rmins.BackwardContext.BackwardToTargetTransitionGUID =
                hasGatewayNode == false ? lastTaskTransitionInstance.TransitionGUID : System.Guid.Empty;
            rmins.BackwardContext.BackwardToTaskActivity = processModel.GetActivity(withdrawActivityInstance.ActivityGUID);
            rmins.BackwardContext.BackwardToTaskActivityInstance = withdrawActivityInstance;
            rmins.BackwardContext.BackwardFromActivity = processModel.GetActivity(runningNode.ActivityGUID);
            rmins.BackwardContext.BackwardFromActivityInstance = runningNode; //准备状态的接收节点
            rmins.BackwardContext.BackwardTaskReciever = WfBackwardTaskReciever.Instance(withdrawActivityInstance.ActivityName,
                withdrawActivityInstance.EndedByUserID.Value,
                withdrawActivityInstance.EndedByUserName);

            //封装AppUser对象
            rmins.AppRunner.AppName = runner.AppName;
            rmins.AppRunner.AppInstanceID = runner.AppInstanceID;
            rmins.AppRunner.UserID = runner.UserID;
            rmins.AppRunner.UserName = runner.UserName;
            rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(withdrawActivityInstance.ActivityGUID,
                runner.UserID,
                runner.UserName);
            rmins.ActivityResource = new ActivityResource(runner, rmins.AppRunner.NextActivityPerformers);

            return rmins;
        }

        /// <summary>
        /// 退回操作
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static WfRuntimeManager CreateRuntimeInstanceSendBack(WfAppRunner runner,
            ref WfSentBackResult result)
        {
            //检查当前运行节点信息
            var rmins = new WfRuntimeManagerSendBack();
            rmins.WfExecutedResult = result = new WfSentBackResult();
            var aim = new ActivityInstanceManager();
            var runningNode = aim.GetRunningNode(runner);
            if (runningNode.ActivityType != (short)ActivityTypeEnum.TaskNode)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfSentBackException.NotTaskNode;
                result.Message = "当前节点不是任务节点，无法退回上一步节点！";
                return rmins;
            }

            if (!(runningNode.ActivityState == (short)ActivityStateEnum.Ready
                || runningNode.ActivityState == (short)ActivityStateEnum.Running))
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfSentBackException.NotInRunning;
                result.Message = string.Format("当前节点的状态不在运行状态，无法退回上一步节点！当前节点状态：{0}",
                    runningNode.ActivityState);
                return rmins;
            }

            if (aim.IsMineTask(runningNode, runner.UserID) == false)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfSentBackException.NotMineTask;
                result.Message = "不是登录用户的任务，无法退回！";
                return rmins;
            }

            var tim = new TransitionInstanceManager();
            var lastTaskTransitionInstance = tim.GetLastTaskTransition(runner.AppName, runner.AppInstanceID, runner.ProcessGUID);
            if (lastTaskTransitionInstance.TransitionType == (short)TransitionTypeEnum.Loop)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfSentBackException.IsLoopNode;
                result.Message = "当前流转是自循环，无需退回！";
                return rmins;
            }

            //设置退回节点的相关信息
            bool hasGatewayNode = false;
            var sendbackToActivityInstance = tim.GetPreviousActivityInstance(runningNode, true,
                out hasGatewayNode).ToList()[0];

            if (sendbackToActivityInstance.ActivityType == (short)ActivityTypeEnum.StartNode)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfSentBackException.IsStartNodePrevious;
                result.Message = "上一步是开始节点，无法退回！";
                return rmins;
            }

            var processModel = (new ProcessModel(runner.ProcessGUID));
            rmins.ProcessModel = processModel;
            rmins.BackwardContext.ProcessInstance = (new ProcessInstanceManager()).GetById(lastTaskTransitionInstance.ProcessInstanceID);
            rmins.BackwardContext.BackwardToTaskActivity = processModel.GetActivity(sendbackToActivityInstance.ActivityGUID);
            rmins.BackwardContext.BackwardToTaskActivityInstance = sendbackToActivityInstance;
            rmins.BackwardContext.BackwardToTargetTransitionGUID =
                hasGatewayNode == false ? lastTaskTransitionInstance.TransitionGUID : System.Guid.Empty;

            rmins.BackwardContext.BackwardFromActivity = processModel.GetActivity(runningNode.ActivityGUID);
            rmins.BackwardContext.BackwardFromActivityInstance = runningNode;
            rmins.BackwardContext.BackwardTaskReciever = WfBackwardTaskReciever.Instance(sendbackToActivityInstance.ActivityName,
                sendbackToActivityInstance.EndedByUserID.Value, sendbackToActivityInstance.EndedByUserName);

            //封装AppUser对象
            rmins.AppRunner.AppName = runner.AppName;
            rmins.AppRunner.AppInstanceID = runner.AppInstanceID;
            rmins.AppRunner.ProcessGUID = runner.ProcessGUID;
            rmins.AppRunner.UserID = runner.UserID;
            rmins.AppRunner.UserName = runner.UserName;
            rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(sendbackToActivityInstance.ActivityGUID,
                sendbackToActivityInstance.EndedByUserID.Value,
                sendbackToActivityInstance.EndedByUserName);
            rmins.ActivityResource = new ActivityResource(runner, rmins.AppRunner.NextActivityPerformers);

            return rmins;
        }

        /// <summary>
        /// 流程返签，先检查约束条件，然后调用wfruntimeinstance执行
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static WfRuntimeManager CreateRuntimeInstanceReverse(WfAppRunner runner,
            ref WfReversedResult result)
        {
            var rmins = new WfRuntimeManagerReverse();
            rmins.WfExecutedResult = result = new WfReversedResult();
            var pim = new ProcessInstanceManager();
            var processInstance = pim.GetProcessInstanceLatest(runner.AppName, runner.AppInstanceID, runner.ProcessGUID);
            if (processInstance == null || processInstance.ProcessState != (short)ProcessStateEnum.Completed)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Exception = WfReversedException.NotInCompleted;
                result.Message = string.Format("当前应用:{0}，实例ID：{1}, 没有完成的流程实例，无法让流程重新运行！",
                    runner.AppName, runner.AppInstanceID);
                return rmins;
            }

            var tim = new TransitionInstanceManager();
            var endTransitionInstance = tim.GetEndTransition(runner.AppName, runner.AppInstanceID, runner.ProcessGUID);

            var processModel = new ProcessModel(runner.ProcessGUID);
            var endActivity = processModel.GetActivity(endTransitionInstance.ToActivityGUID);

            var aim = new ActivityInstanceManager();
            var endActivityInstance = aim.GetById(endTransitionInstance.ToActivityInstanceID);

            bool hasGatewayNode = false;
            var lastTaskActivityInstance = tim.GetPreviousActivityInstance(endActivityInstance, false,
                out hasGatewayNode).ToList()[0];
            var lastTaskActivity = processModel.GetActivity(lastTaskActivityInstance.ActivityGUID);

            //封装返签结束点之前办理节点的任务接收人
            rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(lastTaskActivityInstance.ActivityGUID,
                lastTaskActivityInstance.EndedByUserID.Value,
                lastTaskActivityInstance.EndedByUserName);

            rmins.ActivityResource = new ActivityResource(runner, rmins.AppRunner.NextActivityPerformers);
            rmins.AppRunner.AppName = runner.AppName;
            rmins.AppRunner.AppInstanceID = runner.AppInstanceID;
            rmins.AppRunner.ProcessGUID = runner.ProcessGUID;
            rmins.AppRunner.UserID = runner.UserID;
            rmins.AppRunner.UserName = runner.UserName;

            rmins.BackwardContext.ProcessInstance = processInstance;
            rmins.BackwardContext.BackwardToTaskActivity = lastTaskActivity;
            rmins.BackwardContext.BackwardToTaskActivityInstance = lastTaskActivityInstance;
            rmins.BackwardContext.BackwardToTargetTransitionGUID =
                hasGatewayNode == false ? endTransitionInstance.TransitionGUID : Guid.Empty;
            rmins.BackwardContext.BackwardFromActivity = endActivity;
            rmins.BackwardContext.BackwardFromActivityInstance = endActivityInstance;
            rmins.BackwardContext.BackwardTaskReciever = WfBackwardTaskReciever.Instance(lastTaskActivityInstance.ActivityName,
                lastTaskActivityInstance.EndedByUserID.Value,
                lastTaskActivityInstance.EndedByUserName);

            return rmins;
        }

        #endregion

        #region 运行方法
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        internal bool Execute(IDbSession session)
        {
            try
            {
                ExecuteInstanceImp(session);
            }
            catch (WfRuntimeException rx)
            {
                LogManager.RecordLog(WfDefine.WF_PROCESS_ERROR, LogEventType.Error, LogPriority.High, AppRunner, rx);
                throw;
            }
            catch (System.Exception e)
            {
                LogManager.RecordLog(WfDefine.WF_PROCESS_ERROR, LogEventType.Error, LogPriority.High, AppRunner, e);
                throw;
            }
            finally
            {
                Callback(GetRuntimeManagerType(), WfExecutedResult);
            }

            return true;
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        /// <param name="runtimeType"></param>
        /// <param name="result"></param>
        internal void Callback(RuntimeManagerType runtimeType, WfExecutedResult result)
        {
            WfEventArgs args = new WfEventArgs(result);
            
            if (runtimeType == RuntimeManagerType.StartupRuntime && _onWfProcessStarted != null)
            {
               _onWfProcessStarted(this, args);
            }
            else if (runtimeType == RuntimeManagerType.RunningRuntime && _onWfProcessContinued != null)
            {
                _onWfProcessContinued(this, args);
            }
            else if (runtimeType == RuntimeManagerType.WithdrawRuntime && _onWfProcessWithdrawed != null)
            {
                _onWfProcessWithdrawed(this, args);
            }
            else if (runtimeType == RuntimeManagerType.SendBackRuntime && _onWfProcessSentBack != null)
            {
                _onWfProcessSentBack(this, args);
            }
            else if (runtimeType == RuntimeManagerType.ReverseRuntime && _onWfProcessReversed != null)
            {
                _onWfProcessReversed(this, args);
            }
        }
        #endregion

        #region 流程事件定义
        /// <summary>
        /// 流程被创建事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessStarted;
        internal event EventHandler<WfEventArgs> OnWfProcessStarted
        {
            add
            {
                _onWfProcessStarted += value;
            }
            remove
            {
                _onWfProcessStarted -= value;
            }
        }

        /// <summary>
        /// 流程执行事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessContinued;
        internal event EventHandler<WfEventArgs> OnWfProcessContinued
        {
            add
            {
                _onWfProcessContinued += value;
            }
            remove
            {
                _onWfProcessContinued -= value;
            }
        }

        /// <summary>
        /// 流程撤销回上一步事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessWithdrawed;
        internal event EventHandler<WfEventArgs> OnWfProcessWithdrawed
        {
            add
            {
                _onWfProcessWithdrawed += value;
            }
            remove
            {
                _onWfProcessWithdrawed -= value;
            }
        }

        /// <summary>
        /// 流程退回上一步事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessSentBack;
        internal event EventHandler<WfEventArgs> OnWfProcessSentBack
        {
            add
            {
                _onWfProcessSentBack += value;
            }
            remove
            {
                _onWfProcessSentBack -= value;
            }
        }

        /// <summary>
        /// 流程返签，重新运行事件
        /// </summary>
        private event EventHandler<WfEventArgs> _onWfProcessReversed;
        internal event EventHandler<WfEventArgs> OnWfProcessReversed
        {
            add
            {
                _onWfProcessReversed += value;
            }
            remove
            {
                _onWfProcessReversed -= value;
            }
        }
        #endregion
    }
}

            