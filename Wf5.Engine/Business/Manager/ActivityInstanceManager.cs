using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;

namespace Wf5.Engine.Business.Manager
{
    /// <summary>
    /// 活动实例管理类
    /// </summary>
    internal class ActivityInstanceManager
    {
        #region ActivityInstanceManager 属性

        private Repository _activityInstanceRepository;
        private Repository ActivityInstanceRepository
        {
            get
            {
                if (_activityInstanceRepository == null)
                {
                    _activityInstanceRepository = RepositoryFactory.CreateRepository();
                }
                return _activityInstanceRepository;
            }
        }
        #endregion
       
        #region ActivityInstanceManager 构造函数
        internal ActivityInstanceManager()
        {
        }
        #endregion

        #region ActivityInstanceManager 活动实例数据获取
        internal ActivityInstanceEntity GetById(int activityInstanceID)
        {
            try
            {
                return ActivityInstanceRepository.GetById<ActivityInstanceEntity>(activityInstanceID);
            }
            catch (System.Exception e)
            {
                throw new ApplicationException(string.Format("数据读取方法GetById发生错误，请查看内部异常: {0}", 
                    e.Message), e);
            }
        }

        internal ActivityInstanceEntity GetById(IDbConnection conn, int activityInstanceID, IDbTransaction trans)
        {
            return ActivityInstanceRepository.GetById<ActivityInstanceEntity>(conn, activityInstanceID, trans);
        }

        /// <summary>
        /// 获取流程当前运行节点信息
        /// </summary>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity GetRunningNode(WfAppRunner runner)
        {
            int appInstanceID = runner.AppInstanceID;
            var processGUID = runner.ProcessGUID;
            var taskID = runner.TaskID;

            //如果流程在运行状态，则返回运行时信息
            TaskViewEntity task = null;
            var aim = new ActivityInstanceManager();
            var entity = GetRunningNode(runner, out task);

            return entity;
        }

        internal ActivityInstanceEntity GetRunningNode(WfAppRunner runner, out TaskViewEntity taskView)
        {
            var appInstanceID = runner.AppInstanceID;
            var processGUID = runner.ProcessGUID;
            var taskID = runner.TaskID;

            taskView = null;
            ActivityInstanceEntity activityInstance = null;
           
            //如果流程在运行状态，则返回运行时信息
            var tm = new TaskManager();

            var aim = new ActivityInstanceManager();
            var activityList = aim.GetRunningActivityInstanceList(runner).ToList();

            if ((activityList != null) && (activityList.Count == 1))
            {
                activityInstance = activityList[0];
                taskView = tm.GetTaskOfMine(activityInstance.ActivityInstanceID, runner.UserID);
            }
            else if (activityList.Count > 0)
            {
                if (runner.TaskID != null && runner.TaskID.Value != 0)
                {
                    taskView = tm.GetTaskView(taskID.Value);

                    foreach (var ai in activityList)
                    {
                        if (ai.ActivityInstanceID == taskView.ActivityInstanceID)
                        {
                            activityInstance = ai;
                            break;
                        }
                    }
                }
                else
                {
                    //当前流程运行节点不唯一
                    var e = new WorkflowException("当前流程有多个运行节点，但没有TaskID传入，状态异常！");
                    LogManager.RecordLog("获取当前运行节点信息异常", LogEventType.Exception, LogPriority.Normal, null, e);
                    throw e;
                }
            }
            else
            {
                //当前没有运行状态的节点存在，流程不存在，或者已经结束或取消
                var e = new WorkflowException("当前流程没有运行节点，状态异常！");
                LogManager.RecordLog("获取当前运行节点信息异常", LogEventType.Exception, LogPriority.Normal, null, e);
                throw e;
            }
            return activityInstance;
        }

        /// <summary>
        /// 判断是否是某个用户的办理任务
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal bool IsMineTask(ActivityInstanceEntity entity, int userID)
        {
            bool isMine = entity.AssignedToUsers.Contains(userID.ToString()); 
            return isMine;
        }

        internal IList<ActivityInstanceEntity> GetActivityInstances(int processInstanceID,
            IDbSession session)
        {
            var sql = @"SELECT * FROM WfActivityInstance 
                        WHERE ProcessInstanceID = @processInstanceID 
                            ORDER BY ActivityInstanceID";

            var instanceList = ActivityInstanceRepository.Query<ActivityInstanceEntity>(session.Connection,
                sql,
                new
                {
                    processInstanceID = processInstanceID
                },
                session.Transaction).ToList();

            return instanceList;
        }

        /// <summary>
        /// 判断有活动实例是否在运行状态
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity GetActivityWithRunningState(int processInstanceID,
            Guid activityGUID,
            IDbSession session)
        {
            var sql = @"SELECT * FROM WfActivityInstance 
                        WHERE ProcessInstanceID = @processInstanceID 
                            AND ActivityGUID = @activityGUID 
                            AND ActivityState = @state";

            var instanceList = ActivityInstanceRepository.Query<ActivityInstanceEntity>(session.Connection,
                sql, 
                new
                {
                    processInstanceID = processInstanceID,
                    activityGUID = activityGUID.ToString(),
                    state = (short)ActivityStateEnum.Running
                },
                session.Transaction).ToList();

            if (instanceList.Count == 1)
            {
                return instanceList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 由任务ID获取活动实例信息
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity GetByTask(int taskID, 
            IDbSession session)
        {
            var sql = @"SELECT AI.* FROM WfActivityInstance AI
                        INNER JOIN WfTasks T ON AI.ActivityInstanceID = T.ActivityInstanceID
                        WHERE T.TaskID = @taskID";

            var instanceList = ActivityInstanceRepository.Query<ActivityInstanceEntity>(session.Connection,
                sql,
                new
                {
                    taskID = taskID
                },
                session.Transaction).ToList();

            if (instanceList != null && instanceList.Count == 1)
            {
                return instanceList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取流程实例中运行的活动节点
        /// </summary>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal IEnumerable<ActivityInstanceEntity> GetRunningActivityInstanceList(WfAppRunner runner)
        {
            //activityState: 1-ready（准备）, 2-running（）运行；
            var whereSql = @"SELECT AI.* FROM WfActivityInstance as AI
                            INNER JOIN WfProcessInstance as PI ON AI.ProcessInstanceID = PI.ProcessInstanceID
                            WHERE PI.ProcessState = 2 
                                AND AI.AppInstanceID = @appInstanceID 
                                AND AI.ProcessGUID = @processGUID
                                AND AI.IsActivityCompleted = 0 
                                AND (AI.ActivityState=1 OR AI.ActivityState=2)";

            var instanceList = ActivityInstanceRepository.Query<ActivityInstanceEntity>(
                whereSql, 
                new
                {
                    appInstanceID = runner.AppInstanceID,
                    processGUID = runner.ProcessGUID.ToString()
                });

            var appRunnerInstanceList = instanceList.Where(a => IsAssignedUserInActivityInstance(a, runner.UserID));
            return appRunnerInstanceList;
        }

        private bool IsAssignedUserInActivityInstance(ActivityInstanceEntity entity,
            int userID)
        {
            var assignedToUsers = entity.AssignedToUsers;
            var userList = assignedToUsers.Split(',');
            var single = userList.FirstOrDefault<string>(a=>a == userID.ToString());
            if (!string.IsNullOrEmpty(single))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取会签节点的多实例节点
        /// </summary>
        /// <param name="mainActivityInstanceID">会签节点</param>
        /// <param name="processInstanceID">流程实例ID</param>
        /// <param name="session">session会话</param>
        /// <returns></returns>
        internal IEnumerable<ActivityInstanceEntity> GetActivityMulitipleInstance(int mainActivityInstanceID,
            int processInstanceID,
            IDbSession session)
        {
            //activityState: 1-ready（准备）, 2-running（）运行；
            var whereSql = @"SELECT * FROM WfActivityInstance 
                            WHERE MIHostActivityInstanceID = @activityInstanceID 
                                AND processInstanceID = @processInstanceID
                            ORDER BY CompleteOrder";

            var instanceList = ActivityInstanceRepository.Query<ActivityInstanceEntity>(
                session.Connection,
                whereSql, 
                new
                {
                    activityInstanceID = mainActivityInstanceID,
                    processInstanceID = processInstanceID
                },
                session.Transaction);

            return instanceList;
        }
        #endregion

        /// <summary>
        /// 创建活动实例的对象
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="processInstance"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity CreateActivityInstanceObject(string appName,
            int appInstanceID,
            int processInstanceID,
            ActivityEntity activity,
            WfAppRunner runner)
        {
            ActivityInstanceEntity instance = new ActivityInstanceEntity();
            instance.ActivityGUID = activity.ActivityGUID;
            instance.ActivityName = activity.ActivityName;
            instance.ActivityType = (short)activity.ActivityType;
            instance.GatewayDirectionTypeID = (short)activity.GatewayDirectionType;
            instance.ProcessGUID = activity.ProcessGUID;
            instance.AppName = appName;
            instance.AppInstanceID = appInstanceID;
            instance.ProcessInstanceID = processInstanceID;
            instance.TokensRequired = 1;
            instance.TokensHad = 1;
            instance.CreatedByUserID = runner.UserID;
            instance.CreatedByUserName = runner.UserName;
            instance.CreatedDateTime = System.DateTime.Now;
            instance.ActivityState = (short)ActivityStateEnum.Ready;
            instance.CanRenewInstance = 0;

            return instance;
        }

        /// <summary>
        /// 根据主节点复制子节点
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity CreateActivityInstanceObject(ActivityInstanceEntity main)
        {
            ActivityInstanceEntity instance = new ActivityInstanceEntity();
            instance.ActivityGUID = main.ActivityGUID;
            instance.ActivityName = main.ActivityName;
            instance.ActivityType = main.ActivityType;
            instance.GatewayDirectionTypeID = main.GatewayDirectionTypeID;
            instance.ProcessGUID = main.ProcessGUID;
            instance.AppName = main.AppName;
            instance.AppInstanceID = main.AppInstanceID;
            instance.ProcessInstanceID = main.ProcessInstanceID;
            instance.TokensRequired = 1;
            instance.TokensHad = 1;
            instance.CreatedByUserID = main.CreatedByUserID;
            instance.CreatedByUserName = main.CreatedByUserName;
            instance.CreatedDateTime = System.DateTime.Now;
            instance.ActivityState = (short)ActivityStateEnum.Ready;
            instance.CanRenewInstance = 0;

            return instance;
        }

        /// <summary>
        /// 创建活动实例的对象
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="processInstance"></param>
        /// <returns></returns>
        internal ActivityInstanceEntity CreateBackwardActivityInstanceObject(string appName,
            int appInstanceID,
            int processInstanceID,
            ActivityEntity activity,
            BackwardTypeEnum backwardType,
            int backSrcActivityInstanceID,
            WfAppRunner runner)
        {
            ActivityInstanceEntity instance = new ActivityInstanceEntity();
            instance.ActivityGUID = activity.ActivityGUID;
            instance.ActivityName = activity.ActivityName;
            instance.ActivityType = (short)activity.ActivityType;
            instance.GatewayDirectionTypeID = (short)activity.GatewayDirectionType;
            instance.ProcessGUID = activity.ProcessGUID;
            instance.AppName = appName;
            instance.AppInstanceID = appInstanceID;
            instance.ProcessInstanceID = processInstanceID;
            instance.BackwardType = (short)backwardType;
            instance.BackSrcActivityInstanceID = backSrcActivityInstanceID;
            instance.TokensRequired = 1;
            instance.TokensHad = 1;
            instance.CreatedByUserID = runner.UserID;
            instance.CreatedByUserName = runner.UserName;
            instance.CreatedDateTime = System.DateTime.Now;
            instance.ActivityState = (short)ActivityStateEnum.Ready;
            instance.CanRenewInstance = 0;

            return instance;
        }

        /// <summary>
        /// 更新活动节点的Token数目
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="runner"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void IncreaseTokensHad(int activityInstanceID,
            WfAppRunner runner,
            IDbSession session)
        {
            ActivityInstanceEntity activityInstance = GetById(activityInstanceID);
            activityInstance.TokensHad += 1;
            Update(activityInstance, session);
        }

        #region 活动实例中间状态设置
        /// <summary>
        /// 活动实例被读取
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="logonUser"></param>
        /// <param name="session"></param>
        internal void SetActivityRead(int activityInstanceID,
            int userID,
            string userName,
            IDbSession session)
        {
            SetActivityState(activityInstanceID, ActivityStateEnum.Running, userID, userName, session);
        }

        /// <summary>
        /// 撤销活动实例
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="runner"></param>
        /// <param name="session"></param>
        internal void Withdraw(int activityInstanceID,
            WfAppRunner runner,
            IDbSession session)
        {
            SetActivityState(activityInstanceID, ActivityStateEnum.Withdrawed, runner.UserID, runner.UserName, session);
        }

        /// <summary>
        /// 退回活动实例
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="runner"></param>
        /// <param name="session"></param>
        internal void SendBack(int activityInstanceID,
            WfAppRunner runner,
            IDbSession session)
        {
            SetActivityState(activityInstanceID, ActivityStateEnum.Sendbacked, runner.UserID, runner.UserName, session);
        }

        /// <summary>
        /// 设置活动实例状态
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="nodeState"></param>
        /// <param name="runner"></param>
        /// <param name="session"></param>
        private void SetActivityState(int activityInstanceID,
            ActivityStateEnum nodeState,
            int userID,
            string userName,
            IDbSession session)
        {
            var activityInstance = GetById(activityInstanceID);
            activityInstance.ActivityState = (short)nodeState;
            activityInstance.LastUpdatedByUserID = userID;
            activityInstance.LastUpdatedByUserName = userName;
            activityInstance.LastUpdatedDateTime = System.DateTime.Now;
            Update(activityInstance, session);
        }

        /// <summary>
        /// 活动实例完成
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="runner"></param>
        /// <param name="session"></param>
        internal void Complete(int activityInstanceID, 
            WfAppRunner runner,
            IDbSession session)
        {
            var activityInstance = GetById(session.Connection, activityInstanceID, session.Transaction);
            activityInstance.ActivityState = (short)ActivityStateEnum.Completed;
            activityInstance.IsActivityCompleted = 1;
            activityInstance.LastUpdatedByUserID = runner.UserID;
            activityInstance.LastUpdatedByUserName = runner.UserName;
            activityInstance.LastUpdatedDateTime = System.DateTime.Now;
            activityInstance.EndedByUserID = runner.UserID;
            activityInstance.EndedByUserName = runner.UserName;
            activityInstance.EndedDateTime = System.DateTime.Now;

            Update(activityInstance, session);
        }            
        #endregion

        internal int Insert(ActivityInstanceEntity entity,
            IDbSession session)
        {
            int newID = ActivityInstanceRepository.Insert(session.Connection, entity, session.Transaction);
            entity.ActivityInstanceID = newID;

            return newID;
        }

        internal void Update(ActivityInstanceEntity entity,
            IDbSession session)
        {
            ActivityInstanceRepository.Update(session.Connection, entity, session.Transaction);
        }

        /// <summary>
        /// 删除活动实例
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Delete(int activityInstanceID,
            IDbSession session = null)
        {
            ActivityInstanceRepository.Delete<ActivityInstanceEntity>(session.Connection, 
                activityInstanceID, 
                session.Transaction);
        }
    }
}
