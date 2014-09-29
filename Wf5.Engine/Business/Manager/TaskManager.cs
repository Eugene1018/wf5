using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Xpdl;

namespace Wf5.Engine.Business.Manager
{
    /// <summary>
    /// 任务管理类：包括任务及任务视图对象
    /// </summary>
    public class TaskManager
    {
        #region TaskManager 属性列表
        private Repository _taskRepository;
        private Repository TaskRepository
        {
            get
            {
                if (_taskRepository == null)
                {
                    _taskRepository = RepositoryFactory.CreateRepository();
                }
                return _taskRepository;
            }
        }

        private Repository _taskViewRepository;
        private Repository TaskViewRepository
        {
            get
            {
                if (_taskViewRepository == null)
                {
                    _taskViewRepository = RepositoryFactory.CreateRepository();
                }
                return _taskViewRepository;
            }
        }

        #endregion

        #region TaskManager 任务分配视图
        public TaskViewEntity GetTaskView(int taskID)
        {
            return TaskViewRepository.GetById<TaskViewEntity>(taskID);
        }

        public TaskEntity GetTask(int taskID)
        {
            return TaskRepository.GetById<TaskEntity>(taskID);
        }


        #region TaskManager 获取当前用户的办理任务
        /// <summary>
        /// 获取当前用户运行中的任务
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IEnumerable<TaskViewEntity> GetRunningTasks(TaskQueryEntity query, out int allRowsCount)
        {
            return GetTasksPaged(query, 2, out allRowsCount);
        }

        /// <summary>
        /// 获取当前用户待办的任务
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IEnumerable<TaskViewEntity> GetReadyTasks(TaskQueryEntity query, out int allRowsCount)
        {
            return GetTasksPaged(query, 1, out allRowsCount);
        } 

        /// <summary>
        /// 获取任务（分页）
        /// </summary>
        /// <param name="query"></param>
        /// <param name="activityState"></param>
        /// <returns></returns>
        private IEnumerable<TaskViewEntity> GetTasksPaged(TaskQueryEntity query, int activityState, out int allRowsCount)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）, 2-running（）运行；

            IDbSession session = SessionFactory.CreateSession();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT Top 100 * FROM vwWfActivityInstanceTasks ");
            sql.AppendFormat(" WHERE ProcessState=2 AND ActivityType=4 AND ActivityState={0} AND AssignedToUserID={1} ",
                activityState, query.UserID);
            if (query.AppInstanceID != 0)
            {
                sql.AppendFormat(" AND AppInstanceID={0} ", query.AppInstanceID);
            }
            if (query.ProcessGUID != Guid.Empty)
            {
                sql.AppendFormat(" AND ProcessGUID='{0}' ", query.ProcessGUID);
            }
            if (!string.IsNullOrEmpty(query.AppName))
            {
                sql.AppendFormat(" AND AppName like '%{0}%' ", query.AppName);
            }
            sql.Append(" ORDER BY TASKID DESC "); ;

            //如果数据记录数为0，则不用查询列表
            allRowsCount = TaskRepository.Count<TaskViewEntity>(session.Connection, sql.ToString());
            if (allRowsCount == 0)
            {
                return null;
            }

            //查询列表数据并返回结果集
            var list = TaskRepository.Query<TaskViewEntity>(sql.ToString(),
                new
                {
                    appInstanceID = query.AppInstanceID,
                    processGUID = query.ProcessGUID,
                    activityState = activityState,
                    userID = query.UserID
                });

            return list;
        }

        internal TaskViewEntity GetTaskOfMine(int activityInstanceID, int userID)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）, 2-running（）运行；
            IDbSession session = SessionFactory.CreateSession();
            string whereSql = @"SELECT * FROM vwWfActivityInstanceTasks 
                           WHERE ActivityInstanceID=@activityInstanceID 
                                AND AssignedToUserID=@userID 
                                AND ProcessState=2 AND (ActivityType=4 OR ActivityType=5) 
                                AND (ActivityState=1 OR ActivityState=2) 
                           ORDER BY TASKID DESC";

            var list = TaskRepository.Query<TaskViewEntity>(
                session.Connection,
                whereSql,
                new
                {
                    activityInstanceID = activityInstanceID,
                    userID = userID
                },
                session.Transaction).ToList<TaskViewEntity>();

            //取出唯一待办任务记录，并返回。
            TaskViewEntity task = null;
            if (list != null && list.Count == 1)
            {
                task = list[0];
            }
            return task;
        }

        internal IEnumerable<TaskViewEntity> GetTaskOfMine(int appInstanceID,
            Guid processGUID,
            int userID)
        {
            IDbSession session = SessionFactory.CreateSession();
            return GetTaskOfMine(appInstanceID, processGUID, userID, session);
        }

        /// <summary>
        /// 根据应用实例、流程GUID，办理用户Id获取任务列表
        /// </summary>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="userID"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal IEnumerable<TaskViewEntity> GetTaskOfMine(int appInstanceID, 
            Guid processGUID, 
            int userID, 
            IDbSession session)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）, 2-running（）运行；
            string sql = @"SELECT * FROM vwWfActivityInstanceTasks 
                           WHERE AppInstanceID=@appInstanceID 
                                AND ProcessGUID=@processGUID 
                                AND AssignedToUserID=@userID 
                                AND ProcessState=2 
                                AND ActivityType=4 
                                AND (ActivityState=1 OR ActivityState=2) 
                           ORDER BY TASKID DESC";
            var list = TaskRepository.Query<TaskViewEntity>(session.Connection,
                sql,
                new
                {
                    appInstanceID = appInstanceID,
                    processGUID = processGUID,
                    userID = userID
                },
                session.Transaction);

            return list;
        }

         /// <summary>
        /// 判断活动实例是否是某个用户的任务
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="userID"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal bool IsMine(int activityInstanceID, int userID)
        {
            string whereSql = @"WHERE ActivityInstanceID=@activityInstanceID
                             AND AssignedToUserID=@assignedToUserID";

            var list = TaskRepository.Query<TaskEntity>(whereSql,
                new
                {
                    activityInstanceID = activityInstanceID,
                    assignedToUserID = userID
                }).ToList<TaskEntity>();

            bool isMine = false;
            if (list != null && list.Count == 1)
            {
                isMine = true;
            }
            return isMine;
        }

        internal IEnumerable<TaskViewEntity> GetReadyTaskOfApp(WfAppRunner runner)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）
            string sql = @"SELECT * FROM vwWfActivityInstanceTasks 
                           WHERE AppInstanceID=@appInstanceID 
                                    AND ProcessGUID=@processGUID 
                                    AND ProcessState=2 AND ActivityType=4 AND ActivityState=1";

            var list = TaskRepository.Query<TaskViewEntity>(sql,
                new
                {
                    appInstanceID = runner.AppInstanceID,
                    processGUID = runner.ProcessGUID
                });
            return list;
        }
        #endregion


        /// <summary>
        /// 获取流程实例下的任务数据
        /// </summary>
        /// <param name="appInstanceID">应用ID</param>
        /// <param name="ProcessInstanceID">流程实例ID</param>
        /// <returns>任务列表数据</returns>
        internal IEnumerable<TaskViewEntity> GetProcessTasks(int appInstanceID,
            int processInstanceID)
        {
            string whereSql = @"WHERE ApplicationInstaceID=@appInstanceID 
                            AND ProcessInstanceID=@processInstanceID";
            var list = TaskRepository.Query<TaskViewEntity>(whereSql,
                new
                {
                    appInstanceID = appInstanceID,
                    processInstanceID = processInstanceID
                });
            return list;

        }

        internal IEnumerable<TaskViewEntity> GetProcessTasksWithState(int appInstanceID,
            int processInstanceID,
            ActivityStateEnum state)
        {
            string whereSql = @"WHERE ApplicationInstaceID=@appInstanceID 
                            AND ProcessInstanceID=@processInstanceID 
                            AND ActivityState=@state";
            var list = TaskRepository.Query<TaskViewEntity>(whereSql,
                new
                {
                    appInstanceID = appInstanceID,
                    processInstanceID = processInstanceID,
                    state = state
                });
            return list;
        }

       
        #endregion

        #region TaskManager 任务数据基本操作
        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="entity">任务实体</param>
        /// <param name="wfLinqDataContext">linq上下文</param>
        internal void Insert(TaskEntity entity, 
            IDbSession session)
        {
            int result = TaskRepository.Insert(session.Connection, entity, session.Transaction);
            Debug.WriteLine(string.Format("task instance inserted, time:{0}", System.DateTime.Now.ToString()));
        }

        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <param name="performers"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Insert(ActivityInstanceEntity activityInstance,
            PerformerList performers, 
            WfAppRunner runner,
            IDbSession session)
        {
            foreach (Performer performer in performers)
            {
                Insert(activityInstance, performer, runner, session);
            }
        }

        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <param name="performer"></param>
        /// <param name="runner"></param>
        /// <param name="session"></param>
        internal void Insert(ActivityInstanceEntity activityInstance,
            Performer performer,
            WfAppRunner runner,
            IDbSession session)
        {
            Insert(activityInstance, performer.UserID, performer.UserName, 
                runner.UserID, runner.UserName, session);
        }

        /// <summary>
        /// 插入任务数据(创建任务)
        /// </summary>
        /// <param name="activityInstance"></param>
        /// <param name="performerID"></param>
        /// <param name="performerName"></param>
        /// <param name="runnerID"></param>
        /// <param name="runnerName"></param>
        /// <param name="session"></param>
        private void Insert(ActivityInstanceEntity activityInstance,
            int performerID,
            string performerName,
            int runnerID,
            string runnerName,
            IDbSession session)
        {

            TaskEntity entity = new TaskEntity();
            entity.AppName = activityInstance.AppName;
            entity.AppInstanceID = activityInstance.AppInstanceID;
            entity.ActivityInstanceID = activityInstance.ActivityInstanceID;
            entity.ProcessInstanceID = activityInstance.ProcessInstanceID;
            entity.ActivityGUID = activityInstance.ActivityGUID;
            entity.ActivityName = activityInstance.ActivityName;
            entity.ProcessGUID = activityInstance.ProcessGUID;
            entity.TaskType = (short)TaskTypeEnum.Manual;
            entity.AssignedToUserID = performerID;
            entity.AssignedToUserName = performerName;
            entity.TaskState = 1; //1-待办状态
            entity.CreatedByUserID = runnerID;
            entity.CreatedByUserName = runnerName;
            entity.CreatedDateTime = System.DateTime.Now;
            entity.RecordStatusInvalid = 0;
            //插入任务数据
            Insert(entity, session);
        }

        /// <summary>
        /// 更新任务数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Update(TaskEntity entity, IDbSession session)
        {
            TaskRepository.Update(session.Connection, entity, session.Transaction);
        }


        /// <summary>
        /// 读取任务，设置任务为已读状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        internal void SetTaskRead(WfAppRunner taskRunner)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                session.BeginTransaction();

                //置任务为处理状态
                var task = GetTask(taskRunner.TaskID.Value);
                SetTaskState(task, taskRunner.UserID, taskRunner.UserName, TaskStateEnum.Handling, session);

                //置活动为运行状态
                (new ActivityInstanceManager()).SetActivityRead(task.ActivityInstanceID, taskRunner.UserID, taskRunner.UserName, session);

                session.CommitTransaction();
            }
            catch (System.Exception e)
            {
                session.RollbackTransaction();
                throw new WorkflowException(string.Format("阅读待办任务时出错！，详细错误：{0}", e.Message), e);
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 设置任务状态
        /// </summary>
        /// <param name="task"></param>
        /// <param name="logonUser"></param>
        /// <param name="taskState"></param>
        /// <param name="session"></param>
        private void SetTaskState(TaskEntity task,
            int userID,
            string userName,
            TaskStateEnum taskState,
            IDbSession session)
        {
            task.TaskState = (short)taskState;
            task.LastUpdatedByUserID = userID;
            task.LastUpdatedByUserName = userName;
            task.LastUpdatedDateTime = System.DateTime.Now;
            Update(task, session);
        }

        /// <summary>
        /// 设置任务完成
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="runner"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Complete(long taskID,
            WfAppRunner runner,
            IDbSession session)
        {
            TaskEntity task = TaskRepository.GetById<TaskEntity>(taskID);
            task.TaskState = (byte)TaskStateEnum.Completed;
            task.EndedDateTime = DateTime.Now;
            task.EndedByUserID = runner.UserID;
            task.EndedByUserName = runner.UserName;

            Update(task, session);
        }

        /// <summary>
        /// 创建新的委托任务
        /// </summary>
        /// <param name="entity"></param>
        internal void Entrust(TaskEntrustedEntity entity)
        {
            var session = SessionFactory.CreateSession();
            try
            {
                //获取活动实例信息
                session.BeginTransaction();

                var am = new ActivityInstanceManager();
                var activityInstance = am.GetByTask(entity.TaskID, session);

                if (activityInstance.ActivityState != (short)ActivityStateEnum.Ready
                    && activityInstance.ActivityState != (short)ActivityStateEnum.Running)
                {
                    throw new WorkflowException("没有可以委托的任务，因为活动实例的状态不在运行状态！");
                }

                //更新AssignedToUsers 信息
                activityInstance.AssignedToUsers = activityInstance.AssignedToUsers + "," + entity.EntrustToUserID;
                am.Update(activityInstance, session);

                //插入委托任务
                Insert(activityInstance, entity.EntrustToUserID, entity.EntrustToUserName,
                    entity.RunnerID, entity.RunnerName, session);

                session.CommitTransaction();
            }
            catch(System.Exception e)
            {
                session.RollbackTransaction();
                throw new WorkflowException("任务委托失败，请查看异常信息！", e);
            }
            finally
            {
                session.Dispose();
            }
        }

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="taskID">任务ID</param>
        internal bool Delete(IDbConnection conn, long taskID, IDbTransaction trans)
        {
           return TaskRepository.Delete<TaskEntity>(conn, taskID, trans);
        }
        #endregion
    }
}
