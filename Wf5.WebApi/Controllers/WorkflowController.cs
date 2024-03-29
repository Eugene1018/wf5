﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.Http;
using System.Web.Http.Controllers;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Core;
using Wf5.Engine.Core.Result;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Service;
using Wf5.WebApi.Utility;

namespace Wf5.WebApi.Controllers
{
    //普通顺序流程基本测试(顺序,返签,退回,撤销等测试)
    //文件名: price.normal.xml
    //startup process:
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}

    //run process app:
    ////业务员提交办理节点：
    ////下一步是“板房签字”办理节点
    //{"AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","UserID":"10","UserName":"Long","NextActivityPerformers":{"fc8c71c5-8786-450e-af27-9f6a9de8560f":[{"UserID":10,"UserName":"Long"}]}}

    //withdraw process:
    //撤销至上一步节点（由板房签字到上一步业务员提交）
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}

    //runprocess app
    //下一步是业务员签字
    //{"AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","UserID":"10","UserName":"Long","NextActivityPerformers":{"39c71004-d822-4c15-9ff2-94ca1068d745":[{"UserID":10,"UserName":"Long"}]}}

    //财务审批办理节点：
    ////结束节点
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","NextActivityPerformers":{"b70e717a-08da-419f-b2eb-7a3d71f054de":[{"UserID":10,"UserName":"Long"}]}}

    //run sub process
    //有子流程
    //启动子流程
    //{"AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","UserID":"10","UserName":"Long","NextActivityPerformers":{"5fa796f6-2d5d-4ed6-84e2-a7c4e4e6aabc":[{"UserID":10,"UserName":"Long"}]}}

    
    //reverse process:
    //返签
    //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}

    //sendback process
    //退回
    //数据格式同返签(撤销,退回,返签Json数据格式相同.)

    //read task, and make activity running:
    //任务阅读：
    //{"UserID":"10","UserName":"Long","TaskID":"17"}}

    //获取下一步办理步骤：
    //1) 根据应用来获取
    //GetNextSteps
    //{"AppName":"SamplePrice","AppInstanceID":915,"UserID":"10","UserName":"Long","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d","NextActivityPerformers":{"39c71004-d822-4c15-9ff2-94ca1068d745":[{"UserID":"10","UserName":"Long"}]},"Flowstatus":"启动"}

    //2) 根据任务ID来获取
    //GetTaskNextSteps

    //撤销流程: WithdrawProcess
    //退回流程：SendBackProcess
    //返签流程：ReverseProcess
    //取消运行流程：CancelProcess
    //废弃所有流程实例：DiscardProcess
    /// <summary>
    /// </summary>
    public class WorkflowController  : ApiController
    {
        #region Workflow 数据访问基本操作
        [HttpGet]
        [AllowAnonymous]
        public string Hello()
        {
            return "Hello World!";
        }

        // GET: /Workflow/
        [HttpGet]
        [AllowAnonymous]
        public object Get()
        {
            ProcessManager pm = new ProcessManager();
            var process = pm.GetById(Guid.Parse("072AF8C3-482A-4B1C-890B-685CE2FCC75D"));

            return process;
        }

        [HttpPost]
        [AllowAnonymous]
        public void InsertProcess(ProcessEntity obj)
        {
            ProcessManager pm = new ProcessManager();
            pm.Insert(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        public void UpdateProcess(ProcessEntity obj)
        {
            ProcessManager pm = new ProcessManager();
            pm.Update(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        public void RemoveProcess(Guid processGUID)
        {
            ProcessManager pm = new ProcessManager();
            pm.Delete(processGUID);
        }

        [HttpGet]
        [AllowAnonymous]
        public ProcessInstanceEntity GetProcessInstance()
        {
            IWorkflowService service = new WorkflowService();
            var instance = service.GetProcessInstance(14001);

            return instance;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActivityInstanceEntity GetActivityInstance()
        {
            IWorkflowService service = new WorkflowService();
            var instance = service.GetActivityInstance(100);

            return instance;
        }
        #endregion

        #region Workflow Api访问操作
        [HttpPost]
        [AllowAnonymous]
        public ResponseResult StartProcess(WfAppRunner starter)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                WfStartedResult result = wfService.StartProcess(conn, starter, trans);
                trans.Commit();

                int newProcessInstanceID = result.ProcessInstanceID;
                IList<NodeView> nextStpes = wfService.GetNextActivityTree(starter);

                if (result.Status == WfExecutedStatus.Success)
                {
                    return ResponseResult.Success();
                }
                else
                {
                    return ResponseResult.Error(result.Message);
                }
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return ResponseResult.Error(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult RunProcessApp(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.RunProcessApp(conn, runner, trans);
                trans.Commit();

                if (result.Status == WfExecutedStatus.Success)
                    return ResponseResult.Success();
                else
                    return ResponseResult.Error(result.Message);
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return ResponseResult.Error(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult WithdrawProcess(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.WithdrawProcess(conn, runner, trans);
                trans.Commit();

                if (result.Status == WfExecutedStatus.Success)
                    return ResponseResult.Success();
                else
                    return ResponseResult.Error(result.Message);
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return ResponseResult.Error(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult SendBackProcess(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.SendBackProcess(conn, runner, trans);
                trans.Commit();

                if (result.Status == WfExecutedStatus.Success)
                    return ResponseResult.Success();
                else
                    return ResponseResult.Error(result.Message);
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return ResponseResult.Error(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult ReverseProcess(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(DBConfig.ConnectionString);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.ReverseProcess(conn, runner, trans);
                trans.Commit();

                if (result.Status == WfExecutedStatus.Success)
                    return ResponseResult.Success();
                else
                    return ResponseResult.Error(result.Message);
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
                return ResponseResult.Error(w.Message);
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult DiscardProcess(WfAppRunner discarder)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.DiscardProcess(discarder);

            return ResponseResult.Success();
        }
        #endregion

        #region 任务数据读取操作
        [HttpPost]
        [AllowAnonymous]
        public ResponseResult GetRunningTasks(TaskQueryEntity query)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.GetRunningTasks(query);

            return ResponseResult.Success();
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult GetReadyTasks(TaskQueryEntity query)
        {
            IWorkflowService service = new WorkflowService();
            var result = service.GetReadyTasks(query);

            return ResponseResult.Success();
        }
        #endregion

        #region 流程运行数据读取
        /// <summary>
        /// 获取下一步办理的节点
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ResponseResult<IList<NodeView>> GetTaskNextSteps(int id)
        {
            IWorkflowService wfService = new WorkflowService();
            var nodeList = wfService.GetNextActivityTree(id);

            return ResponseResult<IList<NodeView>>.Success(nodeList);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult<IList<NodeView>> GetNextSteps(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            var nodeList = wfService.GetNextActivityTree(runner);

            return ResponseResult<IList<NodeView>>.Success(nodeList);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult<IList<NodeView>> GetNextStepsContainer(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IUserRoleService roleService = new Wf5.BizApp.UserRoleService();
            var nodeList = wfService.GetNextActivityTree(runner, null, roleService);

            return ResponseResult<IList<NodeView>>.Success(nodeList);
        }

        [HttpGet]
        [AllowAnonymous]
        public ResponseResult<IDictionary<Guid, PerformerList>> GetNextActivityPerformers()
        {
            var performers = new PerformerList();
            performers.Add(new Performer(10, "Long"));

            IDictionary<Guid, PerformerList> nexts = new Dictionary<Guid, PerformerList>();
            nexts[Guid.Parse("10f7481a-ad1a-40f6-aeaa-8d32ceb1fcab")] = performers;

            return ResponseResult<IDictionary<Guid, PerformerList>>.Success(nexts);
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult ReadTask(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            bool isRead = wfService.SetTaskRead(runner);

            return ResponseResult.Success();
        }

        [HttpPost]
        [AllowAnonymous]
        public ResponseResult GetTaskPerformers(WfAppRunner runner)
        {
            IWorkflowService service = new WorkflowService();
            var performers = service.GetTaskPerformers(runner);

            return ResponseResult.Success(performers.Count.ToString());
        }


        /// <summary>
        /// 获取流程实例下所有活动节点的测试示例： 
        ///http://localhost/wf5/api/Workflow/GetActivityInstances/16137
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ResponseResult GetActivityInstances(int id)
        {
            //id -- processInstanceID
            IWorkflowService service = new WorkflowService();
            var instanceList = service.GetActivityInstances(id);

            return ResponseResult.Success(instanceList.Count.ToString());
        }

        /// <summary>
        /// http://localhost/wf5/api/Workflow/EntrustTask
        /// { "TaskID":"18240","RunnerID":"10","RunnerName":"Long","EntrustToUserID":"20","EntrustToUserName":"Zhang" }
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ResponseResult EntrustTask(TaskEntrustedEntity entity)
        {
            IWorkflowService service = new WorkflowService();
            service.EntrustTask(entity);

            return ResponseResult.Success();
        }
        #endregion
    }
}
