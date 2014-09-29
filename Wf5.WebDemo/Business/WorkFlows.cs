using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

using Wf5.WebDemo.Entity;
using Wf5.WebDemo.Data;
using Wf5.WebDemo.Common;


using Wf5.Engine;
using Wf5.Engine.Business;
using Wf5.Engine.Common;
using Wf5.Engine.Core;
using Wf5.Engine.Utility;
using Wf5.Engine.Service;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Core.Result;


namespace Wf5.WebDemo.Business
{
    public class WorkFlows
    {
        /// <summary>
        /// 获取系统中的角色列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRoleList()
        {
            return Wf5.WebDemo.Data.WorkFlowManager.GetRoleList();
        }

        /// <summary>
        /// 根据角色ID查询用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static DataTable GetUserListByRoleId(int roleId)
        {
            return Wf5.WebDemo.Data.WorkFlowManager.GetUserListByRoleId(roleId);
        }

        /// <summary>
        /// 根据角色ID查询用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static DataTable GetUserListByRoleIdList(string roleIdList)
        {
            return Wf5.WebDemo.Data.WorkFlowManager.GetUserListByRoleIdList(roleIdList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public static Wf5.WebDemo.Entity.UserInfo GetUserModel(int uId)
        {
            return Wf5.WebDemo.Data.WorkFlowManager.GetUserModel(uId);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static FlowInstanceInfo GetFlowInstanceModel(int fiID)
        {
            return Wf5.WebDemo.Data.WorkFlowManager.GetFlowInstanceModel(fiID);
        }

         /// <summary>
        /// 获得办理意见集合
        /// </summary>
        /// <param name="instanceId"></param>
        public static DataTable GetFlowOpinionList(int instanceId)
        {
            return Wf5.WebDemo.Data.WorkFlowManager.GetFlowOpinionList(instanceId);
        }

        /// <summary>
        /// 查询当前流程办理情况列表
        /// </summary>
        /// <param name="Action">Action: flowTodo-待办 flowHasdo-已办</param>
        /// <returns></returns>
        public static string GetReadTasksDataList(string flag)
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(" <table width=\'100%\' border=\'0\' cellpadding=\'0\' cellspacing=\'0\' class=\'tblist\'> ");
            sb.Append("<thead>                                                                                   ");
            sb.Append("    <tr>                                                                                  ");
            sb.Append("        <th align=\"center\">序号</th>                                                    ");
            sb.Append("        <th align=\"center\">流程编号/自定义标题</th>                                     ");
            //sb.Append("        <th align=\"center\">所属分类/所属流程</th>                                       ");
            if (flag == "flowTodo")//待办
            {
                sb.Append("        <th align=\"center\">发起人/发起时间</th>                                     ");
            }
            else if (flag == "flowHasdo")//已办
            {
                sb.Append("        <th align=\"center\">发起/结束时间</th>                                       ");
            }
            sb.Append("        <th align=\"center\">重要等级</th>                                                ");
            sb.Append("        <th align=\"center\">状态</th>                                                    ");
            sb.Append("        <th align=\"center\">申请人</th>                                                  ");
            sb.Append("        <th align=\"center\">申请时间</th>                                                ");
            sb.Append("        <th align=\"center\">操作</th>                                                    ");
            sb.Append("    </tr>                                                                                 ");
            sb.Append("</thead>                                                                                  ");
            sb.Append("<tbody>");

            if (!string.IsNullOrEmpty(flag))
            {
                DataTable dt = Wf5.WebDemo.Data.WorkFlowManager.GetReadTasks(Wf5.WebDemo.Common.Helper.GetLoginUserId());
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>");
                    sb.AppendFormat("<td align=\"center\">{0}</td>", i);
                    sb.AppendFormat("<td align=\"left\">{0}<br/>{1}</td>", dr["fiNo"], dr["fiTitle"]);
                    // sb.AppendFormat("<td align=\"left\">{0}<br/>{1}</td>", dr["ftName"], dr["fdName"]);

                    if (flag == "flowTodo")//待办
                    {
                        sb.AppendFormat("<td align=\"left\">{0}<br/>{1}</td>", dr["fiApplyer"], dr["fiApplyTime"]);
                    }
                    else if (flag == "flowHasdo")//已办
                    {
                        sb.AppendFormat("<td align=\"left\">{0}<br/>{1}</td>", dr["CreatedDateTime"], dr["EndedDateTime"]);
                    }
                    string level = dr["fiLevel"] == null ? "" : dr["fiLevel"].ToString();
                    string levelName = string.Empty;
                    if (level == "0")
                    {
                        levelName = "普通";
                    }
                    else if (level == "1")
                    {
                        levelName = "重要";
                    }
                    else if (level == "2")
                    {
                        levelName = "非常重要";
                    }
                    sb.AppendFormat("<td align=\"left\">{0}</td>", levelName);
                    sb.AppendFormat("<td align=\"left\">{0}</td>", dr["IsActivityCompleted"].ToString() == "0" ? "办理中" : "结束");

                    sb.AppendFormat("<td align=\"left\">{0}</td>", dr["fiApplyer"].ToString());
                    sb.AppendFormat("<td align=\"left\">{0}</td>", dr["fiApplyTime"]);
                    sb.Append("<td align=\"center\">");

                    if (flag == "flowTodo")//待办
                    {
                        sb.Append("<a href=\"FlowApproval.aspx?Action=FlowReadOnly&&instanceId=" + dr["fiID"] + "\" target=\"_blank\">查看</a>|");
                        sb.Append("<a href=\"FlowApproval.aspx?Action=FlowApproval&&instanceId=" + dr["fiID"] + "\" target=\"_blank\">办理</a>");
                    }
                    else if (flag == "flowHasdo")//已办
                    {
                        sb.Append("<a href=\"FlowApproval.aspx?Action=FlowReadOnly&&instanceId=" + dr["fiID"] + "\" target=\"_blank\">查看</a>");
                    }
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    i++;
                }
            }
            sb.Append("</tbody> <tfoot></tfoot></table>");
            return sb.ToString();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="processGuid"></param>
        /// <param name="operatorId"></param>
        /// <param name="instanceId"></param>
        /// <param name="flowRecordInfoList"></param>
        private static void MakeFlowRecordInfo(Guid processGuid, int operatorId, int instanceId, string approvalMemo, ref List<Wf5.WebDemo.Entity.FlowRecordInfo> flowRecordInfoList)
        {
            Wf5.WebDemo.Entity.FlowRecordInfo flowRecordInfo;
            try
            {
                IWorkflowService service = new WorkflowService();

                WfAppRunner runner = new WfAppRunner();
                runner.ProcessGUID = processGuid;
                runner.UserID = operatorId;
                runner.AppInstanceID = instanceId;
                ActivityInstanceEntity activityInstanceEntity = service.GetRunningNode(runner);
                if (activityInstanceEntity != null)
                {
                    flowRecordInfo = new Wf5.WebDemo.Entity.FlowRecordInfo();
                    flowRecordInfo.frProcessGuid = processGuid;
                    flowRecordInfo.frProcessInstanceID = activityInstanceEntity.ProcessInstanceID;
                    flowRecordInfo.frAppInstanceID = activityInstanceEntity.AppInstanceID;
                    flowRecordInfo.frActivityInstanceID = activityInstanceEntity.ActivityInstanceID;
                    flowRecordInfo.frActivityGuid = activityInstanceEntity.ActivityGUID;
                    flowRecordInfo.frActivityType = activityInstanceEntity.ActivityType;
                    flowRecordInfo.frActivityState = activityInstanceEntity.ActivityState;
                    flowRecordInfo.frDealAdvice = approvalMemo == null ? "" : approvalMemo;
                    flowRecordInfo.frFile = "";
                    flowRecordInfo.frSignInfo = "";
                    flowRecordInfo.frOperator = Wf5.WebDemo.Common.Helper.GetLoginUserId();

                    if (flowRecordInfoList != null)
                    {
                        flowRecordInfoList.Add(flowRecordInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 批量保存流程记录数据
        /// </summary>
        /// <param name="flowRecordInfoList"></param>
        /// <returns></returns>
        private static bool SaveWorkFlowFlowRecord(List<Wf5.WebDemo.Entity.FlowRecordInfo> flowRecordInfoList)
        {
            bool result = false;
            try
            {
                if (flowRecordInfoList != null && flowRecordInfoList.Count > 0)
                {
                    return Wf5.WebDemo.Data.WorkFlowManager.SaveFlowFlowRecord(flowRecordInfoList);
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        /// <summary>
        /// 流程发起第一步 保存数据 且流程第一步开始
        /// </summary>
        /// <param name="nextActivityGuid"></param>
        /// <param name="nextActivityUserId"></param>
        /// <param name="wfInstanceInfo"></param>
        /// <param name="rtMessage"></param>
        /// <returns></returns>
        public static bool FlowApply(string nextActivityGuid, int nextActivityUserId, FlowInstanceInfo wfInstanceInfo, ref string rtMessage)
        {
            List<Wf5.WebDemo.Entity.FlowRecordInfo> flowRecordInfoList = new List<Wf5.WebDemo.Entity.FlowRecordInfo>();
            rtMessage = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(nextActivityGuid))
                {
                    int operatorId = Wf5.WebDemo.Common.Helper.GetLoginUserId();
                    string opratorName = Wf5.WebDemo.Common.Helper.GetLoginUserName();

                    wfInstanceInfo.fiApplyer = operatorId.ToString();
                    wfInstanceInfo.fiLastOperator = operatorId.ToString();
                    wfInstanceInfo.fiType = "0";
                    wfInstanceInfo.fiSerialNo = System.Guid.NewGuid().ToString();
                    wfInstanceInfo.fiState = "0";

                    int instanceId = Wf5.WebDemo.Data.WorkFlowManager.FlowApply(wfInstanceInfo);
                    if (instanceId <= 0)
                    {
                        rtMessage = "保存数据失败，无法继续操作！";
                        return false;
                    }

                    //调用流程
                    Guid processGUID = Guid.Parse(wfInstanceInfo.fiFlowDefinitionGuid.ToString());
                    IWorkflowService service = new WorkflowService();
                    WfAppRunner initiator = new Wf5.Engine.Common.WfAppRunner();//流程执行人(业务应用的办理者)
                    initiator.AppName = wfInstanceInfo.fiTitle;
                    initiator.AppInstanceID = instanceId;
                    initiator.ProcessGUID = processGUID;
                    initiator.UserID = operatorId;
                    initiator.UserName = opratorName;
                    //启动流程
                    Wf5.Engine.Core.Result.WfStartedResult startedResult = service.StartProcess(initiator);

                    if (startedResult.Status != WfExecutedStatus.Success)
                    {
                        rtMessage = startedResult.Message;
                        return false;
                    }
                    MakeFlowRecordInfo(processGUID, operatorId, instanceId, "", ref flowRecordInfoList);


                    Wf5.WebDemo.Entity.UserInfo uInfo = new UserInfo();
                    uInfo = GetUserModel(nextActivityUserId);
                    PerformerList pList = new PerformerList();
                    pList.Add(new Performer(uInfo.uID, uInfo.uName));

                    initiator.NextActivityPerformers = new Dictionary<Guid, PerformerList>();
                    initiator.NextActivityPerformers.Add(Guid.Parse(nextActivityGuid), pList);
                    WfRunAppResult runAppResult = service.RunProcessApp(initiator);
                    if (runAppResult.Status != WfExecutedStatus.Success)
                    {
                        rtMessage = startedResult.Message;
                        return false;
                    }

                    MakeFlowRecordInfo(processGUID, operatorId, instanceId, "", ref flowRecordInfoList);

                    SaveWorkFlowFlowRecord(flowRecordInfoList);

                    return true;
                }
                else
                {
                    rtMessage = "流程相关数据为空，数据无法提交！";
                    return false;
                }
            }
            catch (Exception ex)
            {
                rtMessage = ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 流程审批中间步骤处理
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="nextActivityGuid"></param>
        /// <param name="nextActivityUserId"></param>
        /// <param name="approvalMemo"></param>
        /// <param name="rtMessage"></param>
        /// <returns></returns>
        public static bool FlowApproval(int instanceId, string nextActivityGuid, int nextActivityUserId, string approvalMemo, ref string rtMessage)
        {
            rtMessage = string.Empty;
            try
            {
                List<FlowRecordInfo> flowRecordInfoList = new List<FlowRecordInfo>();
                UserInfo userModel = WorkFlowManager.GetUserModel(nextActivityUserId);
                FlowInstanceInfo instanceInfo = GetFlowInstanceModel(instanceId);

                if (!string.IsNullOrEmpty(nextActivityGuid) && userModel != null && instanceInfo != null)
                {
                    int operatorId = Helper.GetLoginUserId();
                    string opratorName = Helper.GetLoginUserName();

                    Guid rocessGUID = instanceInfo.fiFlowDefinitionGuid;

                    //调用流程
                    WfAppRunner initiator = new Wf5.Engine.Common.WfAppRunner();
                    initiator.AppName = instanceInfo.fiTitle;
                    initiator.AppInstanceID = instanceId;
                    initiator.ProcessGUID = rocessGUID;
                    initiator.UserID = operatorId;
                    initiator.UserName = opratorName;

                    IWorkflowService service = new WorkflowService();

                    //人事部经理审批 
                    PerformerList pList = new PerformerList();
                    pList.Add(new Performer(userModel.uID, userModel.uName));

                    initiator.NextActivityPerformers = new Dictionary<Guid, PerformerList>();
                    initiator.NextActivityPerformers.Add(Guid.Parse(nextActivityGuid), pList);
                    service.RunProcessApp(initiator);


                    MakeFlowRecordInfo(rocessGUID, operatorId, instanceId, approvalMemo, ref flowRecordInfoList);
                    SaveWorkFlowFlowRecord(flowRecordInfoList);
                    return true;
                }
                else
                {
                    rtMessage = "流程相关数据为空，数据无法提交！";
                    return false;
                }
            }
            catch (Exception ex)
            {
                rtMessage = ex.ToString();
                return false;
            }
        }
    }
}