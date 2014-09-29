using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Wf5.Engine.Common;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Core;
using Wf5.Engine.Core.Result;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Xpdl.Node;

namespace Wf5.Engine.Service
{
    /// <summary>
    /// 工作流服务接口
    /// </summary>
    public interface IWorkflowService
    {

        /// <summary>
        /// 获取流程的第一个可办理节点[new]
        /// </summary>
        /// <returns></returns>
        ActivityEntity GetFirstActivity(Guid processGUID);

        /// <summary>
        /// 获取当前节点的下一个节点信息[new]
        /// </summary>
        /// <param name="processGUID"></param>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        ActivityEntity GetNextActivity(Guid processGUID, Guid activityGUID);

        /// <summary>
        /// 获取下一步办理节点树
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        IList<NodeView> GetNextActivityTree(int taskID, IDictionary<string, string> condition = null, IUserRoleService roleService = null);

        /// <summary>
        /// 获取下一步办理节点树
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        IList<NodeView> GetNextActivityTree(WfAppRunner runner, IDictionary<string, string> condition = null, IUserRoleService roleService = null);

        /// <summary>
        /// 获取流程的发起人
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <returns></returns>
        Performer GetProcessInitiator(int processInstanceID);

        /// <summary>
        /// 启动历程
        /// </summary>
        /// <param name="starter">启动人</param>
        /// <returns></returns>
        WfStartedResult StartProcess(WfAppRunner runner);

        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="starter">启动人</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        WfStartedResult StartProcess(IDbConnection conn, WfAppRunner starter, IDbTransaction trans);

        /// <summary>
        /// 运行流程（根据业务数据运行流程）
        /// </summary>
        /// <param name="runner">运行者</param>
        /// <returns></returns>
        WfRunAppResult RunProcessApp(WfAppRunner runner);


        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="runner">运行者</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        WfRunAppResult RunProcessApp(IDbConnection conn, WfAppRunner runner, IDbTransaction trans);

        /// <summary>
        /// 流程撤销回当前节点：将下一步节点收回
        /// </summary>
        /// <param name="withdrawer"></param>
        /// <returns></returns>
        WfWithdrawResult WithdrawProcess(WfAppRunner runner);


        /// <summary>
        /// 流程撤销回当前节点：将下一步节点收回
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="withdrawer"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        WfWithdrawResult WithdrawProcess(IDbConnection conn, WfAppRunner withdrawer, IDbTransaction trans);

        /// <summary>
        /// 流程退回上一步：由当前节点发起
        /// </summary>
        /// <param name="rejector"></param>
        /// <returns></returns>
        WfSentBackResult SendBackProcess(WfAppRunner runner);

        /// <summary>
        /// 流程退回上一步：由当前节点发起
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="runner"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        WfSentBackResult SendBackProcess(IDbConnection conn, WfAppRunner runner, IDbTransaction trans);

        /// <summary>
        /// 流程返签
        /// </summary>
        /// <param name="starter">返签人</param>
        /// <returns></returns>
        WfReversedResult ReverseProcess(WfAppRunner runner);

        /// <summary>
        /// 流程返签
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="starter">任务执行者</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        WfReversedResult ReverseProcess(IDbConnection conn, WfAppRunner starter, IDbTransaction trans);

        /// <summary>
        /// 流程取消：运行状态置为取消状态
        /// </summary>
        /// <param name="canceler"></param>
        /// <returns></returns>
        bool CancelProcess(WfAppRunner canceler);

        /// <summary>
        /// 流程废弃：流程由运行、完成状态置为废弃状态
        /// </summary>
        /// <param name="discarder"></param>
        /// <returns></returns>
        bool DiscardProcess(WfAppRunner discarder);
       
        /// <summary>
        /// 设置任务为已阅读(通过任务列表获取任务)
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        bool SetTaskRead(WfAppRunner runner);

        /// <summary>
        /// 获取当前用户运行任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList<TaskViewEntity> GetRunningTasks(TaskQueryEntity query);

        /// <summary>
        /// 获取当前用户待办任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList<TaskViewEntity> GetReadyTasks(TaskQueryEntity query);

        /// <summary>
        /// 委托任务给某人
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        void EntrustTask(TaskEntrustedEntity entrusted);


        /// <summary>
        /// 活动当前运行的节点实例信息
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        ActivityInstanceEntity GetRunningNode(WfAppRunner runner);

        /// <summary>
        /// 获取运行中的流程实例
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        ProcessInstanceEntity GetRunningProcessInstance(WfAppRunner runner);

        /// <summary>
        /// 获取当前办理任务人员列表
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        IList<Performer> GetTaskPerformers(WfAppRunner runner);

        /// <summary>
        /// 获取一个流程实例下的所有活动实例
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <returns></returns>
        IList<ActivityInstanceEntity> GetActivityInstances(int processInstanceID);

        /// <summary>
        /// 获取流程实例数据
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <returns></returns>
        ProcessInstanceEntity GetProcessInstance(int processInstanceID);

        /// <summary>
        /// 获取活动实例数据
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <returns></returns>
        ActivityInstanceEntity GetActivityInstance(int activityInstanceID);
    }
}
