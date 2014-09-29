using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using Wf5.Engine.Core;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Core.Result;
using Wf5.Engine.Core.Event;
using Autofac;

namespace Wf5.Engine.Service
{
    /// <summary>
    /// 工作流服务(执行部分)
    /// </summary>
    public partial class WorkflowService : IWorkflowService
    {
        #region 构造方法
        public WorkflowService()
        {

        }
        #endregion
        #region 获取节点信息
        /// <summary>
        /// 获取流程的第一个可办理节点
        /// </summary>
        /// <returns></returns>
        public ActivityEntity GetFirstActivity(Guid processGUID)
        {
            var processModel = new ProcessModel(processGUID);
            var firstActivity = processModel.GetFirstActivity();
            return firstActivity;
        }

        /// <summary>
        /// 获取当前节点的下一个节点信息
        /// </summary>
        /// <param name="processGUID"></param>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        public ActivityEntity GetNextActivity(Guid processGUID, Guid activityGUID)
        {
            var processModel = new ProcessModel(processGUID);
            var nextActivity = processModel.GetNextActivity(activityGUID);
            return nextActivity;
        }

        /// <summary>
        /// 根据应用获取流程下一步节点列表
        /// </summary>
        /// <param name="runner">应用执行人</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public IList<NodeView> GetNextActivityTree(WfAppRunner runner, 
            IDictionary<string, string> condition = null,
            IUserRoleService roleService = null)
        {
            var tm = new TaskManager();
            var processModel = new ProcessModel(runner.ProcessGUID);
            var session = SessionFactory.CreateSession();
            var taskList = tm.GetTaskOfMine(runner.AppInstanceID, runner.ProcessGUID, runner.UserID, session).ToList();

            if (taskList == null || taskList.Count == 0)
            {
                throw new WorkflowException("没有当前你正在办理的任务，流程无法读取下一步节点！");
            }
            else if (taskList.Count > 1)
            {
                throw new WorkflowException(string.Format("当前应用ID的办理任务数目要唯一，不是正确的任务数目！错误数目：{0}", taskList.Count));
            }

            var task = taskList[0];
            var nextSteps = processModel.GetNextActivityTree(task.ProcessInstanceID,
                task.ActivityGUID,
                condition,
                roleService);

            return nextSteps;
        }

        /// <summary>
        /// 获取下一步活动列表树
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public IList<NodeView> GetNextActivityTree(int taskID, 
            IDictionary<string, string> condition = null,
            IUserRoleService roleService = null)
        {
            var task = (new TaskManager()).GetTaskView(taskID);
            var processModel = new ProcessModel(task.ProcessGUID);
            var nextSteps = processModel.GetNextActivityTree(task.ProcessInstanceID, 
                task.ActivityGUID, 
                condition,
                roleService);

            return nextSteps;
        }
        #endregion

        #region 流程启动
        private AutoResetEvent waitHandler = new AutoResetEvent(false);
        private WfStartedResult _startedResult = null;
        private WfRunAppResult _runAppResult = null;
        private WfWithdrawResult _withdrawedResult = null;
        private WfSentBackResult _sendbackResult = null;
        private WfReversedResult _reversedResult = null;

        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="starter"></param>
        /// <returns></returns>
        public WfStartedResult StartProcess(WfAppRunner starter)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = StartProcess(conn, starter, trans);
                trans.Commit();
                return result;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public WfStartedResult StartProcess(IDbConnection conn, WfAppRunner starter, IDbTransaction trans)
        {
            try
            {
                IDbSession session = SessionFactory.CreateSession(conn, trans);
                var runtimeInstance = WfRuntimeManager.CreateRuntimeInstanceStartup(starter, ref _startedResult);

                if (_startedResult.Status == WfExecutedStatus.Exception)
                {
                    return _startedResult;
                }

                runtimeInstance.OnWfProcessStarted += runtimeInstance_OnWfProcessStarted;
                runtimeInstance.Execute(session);

                //do something else here...

                waitHandler.WaitOne();
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程启动发生错误，内部异常:{0}", e.Message), e);
            }
            catch (WorkflowException)
            {
                throw;
            }

            //查看是否生成新流程实例
            if (_startedResult.ProcessInstanceID == 0)
            {
                throw new WorkflowException("流程启动失败，获取不到运行的流程实例!");
            }
            return _startedResult;
        }

        private void runtimeInstance_OnWfProcessStarted(object sender, WfEventArgs args)
        {
            _startedResult = args.WfExecutedResult as WfStartedResult;
            waitHandler.Set();
        }
        #endregion

        #region 运行流程     
        /// <summary>
        /// 运行流程(业务处理)
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public WfRunAppResult RunProcessApp(WfAppRunner runner)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = RunProcessApp(conn, runner, trans);
                trans.Commit();
                return result;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="runner"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public WfRunAppResult RunProcessApp(IDbConnection conn, WfAppRunner runner, IDbTransaction trans)
        {
            try
            {
                IDbSession session = SessionFactory.CreateSession(conn, trans);
                var runtimeInstance = WfRuntimeManager.CreateRuntimeInstanceAppRunning(runner, ref _runAppResult);

                if (_runAppResult.Status == WfExecutedStatus.Exception)
                {
                    return _runAppResult;
                }

                runtimeInstance.OnWfProcessContinued += runtimeInstance_OnWfProcessContinued;
                bool isRunning = runtimeInstance.Execute(session);

                waitHandler.WaitOne();

                return _runAppResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程运行时发生异常！，详细错误：{0}", e.Message), e);
            }
            catch (WorkflowException)
            {
                throw;
            }
        }

        private void runtimeInstance_OnWfProcessContinued(object sender, WfEventArgs args)
        {
            _runAppResult = args.WfExecutedResult as WfRunAppResult;
            waitHandler.Set();
        }
        #endregion

        #region 流程撤销、回退和返签（已经结束的流程可以被复活）
        ///// <summary>
        ///// 流程撤销
        ///// </summary>
        ///// <param name="recaller"></param>
        ///// <returns></returns>
        public WfWithdrawResult WithdrawProcess(WfAppRunner runner)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = WithdrawProcess(conn, runner, trans);
                trans.Commit();
                return result;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public WfWithdrawResult WithdrawProcess(IDbConnection conn, WfAppRunner withdrawer, IDbTransaction trans)
        {
            try
            {
                IDbSession session = SessionFactory.CreateSession(conn, trans);
                var runtimeInstance = WfRuntimeManager.CreateRuntimeInstanceWithdraw(withdrawer, ref _withdrawedResult);

                //不满足撤销操作，返回异常结果信息
                if (_withdrawedResult.Status == WfExecutedStatus.Exception)
                {
                    return _withdrawedResult;
                }

                runtimeInstance.OnWfProcessWithdrawed += runtimeInstance_OnWfProcessWithdrawed;
                bool isWithdrawed = runtimeInstance.Execute(session);

                waitHandler.WaitOne();

                return _withdrawedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程撤销发生异常！，详细错误：{0}", e.Message), e);
            }
            catch (WorkflowException)
            {
                throw;
            }
        }

        private void runtimeInstance_OnWfProcessWithdrawed(object sender, WfEventArgs args)
        {
            _withdrawedResult = args.WfExecutedResult as WfWithdrawResult;
            waitHandler.Set();
        }

        /// <summary>
        /// 退回流程
        /// </summary>
        /// <param name="rejector"></param>
        /// <returns></returns>
        public WfSentBackResult SendBackProcess(WfAppRunner runner)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = SendBackProcess(conn, runner, trans);
                trans.Commit();
                return result;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public WfSentBackResult SendBackProcess(IDbConnection conn, WfAppRunner sender, IDbTransaction trans)
        {
            try
            {
                IDbSession session = SessionFactory.CreateSession(conn, trans);
                var runtimeInstance = WfRuntimeManager.CreateRuntimeInstanceSendBack(sender, ref _sendbackResult);

                if (_sendbackResult.Status == WfExecutedStatus.Exception)
                {
                    return _sendbackResult;
                }

                runtimeInstance.OnWfProcessSentBack += runtimeInstance_OnWfProcessSentBack;
                bool isRejected = runtimeInstance.Execute(session);

                waitHandler.WaitOne();

                return _sendbackResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程退回发生异常！，详细错误：{0}", e.Message), e);
            }
            catch (WorkflowException)
            {
                throw;
            }
        }

        private void runtimeInstance_OnWfProcessSentBack(object sender, WfEventArgs args)
        {
            _sendbackResult = args.WfExecutedResult as WfSentBackResult;
            waitHandler.Set();
        }

        /// <summary>
        /// 流程返签
        /// </summary>
        /// <param name="ender"></param>
        /// <returns></returns>
        public WfReversedResult ReverseProcess(WfAppRunner runner)
        {
            IDbConnection conn = SessionFactory.CreateConnection();
            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = ReverseProcess(conn, runner, trans);
                trans.Commit();
                return result;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public WfReversedResult ReverseProcess(IDbConnection conn, WfAppRunner ender, IDbTransaction trans)
        {
            try
            {
                IDbSession session = SessionFactory.CreateSession(conn, trans);
                var runtimeInstance = WfRuntimeManager.CreateRuntimeInstanceReverse(ender, ref _reversedResult);

                if (_reversedResult.Status == WfExecutedStatus.Exception)
                {
                    return _reversedResult;
                }

                runtimeInstance.OnWfProcessReversed += runtimeInstance_OnWfProcessReversed;
                bool isReversed = runtimeInstance.Execute(session);

                waitHandler.WaitOne();

                //return _wfExecutedResult;
                return _reversedResult;
            }
            catch (WfRuntimeException e)
            {
                throw new WorkflowException(string.Format("流程返签发生异常！，详细错误：{0}", e.Message), e);
            }
            catch (WorkflowException)
            {
                throw;
            }
        }

        private void runtimeInstance_OnWfProcessReversed(object sender, WfEventArgs args)
        {
            _reversedResult = args.WfExecutedResult as WfReversedResult;
            waitHandler.Set();
        }
        #endregion

        #region 取消（运行的）流程、废弃执行中或执行完的流程
        /// <summary>
        /// 取消流程
        /// </summary>
        /// <param name="canceller"></param>
        /// <returns></returns>
        public bool CancelProcess(WfAppRunner runner)
        {
            var pim = new ProcessInstanceManager();
            return pim.Cancel(runner);
        }

        /// <summary>
        /// 废弃流程
        /// </summary>
        /// <param name="discarder"></param>
        /// <returns></returns>
        public bool DiscardProcess(WfAppRunner runner)
        {
            var pim = new ProcessInstanceManager();
            return pim.Discard(runner);
        }
        #endregion

        #region 任务读取和处理
        /// <summary>
        /// 设置任务为已读状态(根据任务ID获取任务)
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public bool SetTaskRead(WfAppRunner taskRunner)
        {
            bool isRead = false;
            try
            {
                var taskManager = new TaskManager();
                taskManager.SetTaskRead(taskRunner);
                isRead = true;
            }
            catch (System.Exception)
            {
                throw;
            }

            return isRead;
        }

        public IList<TaskViewEntity> GetRunningTasks(TaskQueryEntity query)
        {
            int allRowsCount = 0;
            var taskManager = new TaskManager();
            var taskList = taskManager.GetRunningTasks(query, out allRowsCount);
            if (taskList != null)
                return taskList.ToList();
            else
                return null;
        }

        public IList<TaskViewEntity> GetReadyTasks(TaskQueryEntity query)
        {
            int allRowsCount = 0;
            var taskManager = new TaskManager();
            var taskList = taskManager.GetReadyTasks(query, out allRowsCount);

            if (taskList != null)
                return taskList.ToList();
            else
                return null;
        }
        #endregion
    }
}