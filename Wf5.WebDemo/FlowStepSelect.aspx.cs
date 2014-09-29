using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Text;
using System.Data;


using Wf5.Engine.Common;
using Wf5.Engine.Service;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Business.Entity;

namespace Wf5.WebDemo
{
    public partial class FlowStepSelect : System.Web.UI.Page
    {
        public string stepList = string.Empty;
        public string userList = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitStep();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitStep()
        {
            //流程定义的GUID
            string flowGuid = Request.QueryString["flowGuid"] == null ? "" : Request.QueryString["flowGuid"].ToString();
            string Step = Request.QueryString["Step"] == null ? "" : Request.QueryString["Step"].ToString();
            if (string.IsNullOrEmpty(flowGuid) || string.IsNullOrEmpty(Step))
            {
                base.RegisterStartupScript("", "<script>alert('参数为空,数据无法加载！ ');</script>");
            }
            else
            {
                Guid processGUID = Guid.Parse(flowGuid);
                IWorkflowService service = new WorkflowService();
                switch (Step)
                {
                    case "start"://流程第一步选择
                        Wf5.Engine.Xpdl.ActivityEntity firstActivity = service.GetFirstActivity(processGUID);
                        Guid firstActivityGUID = firstActivity.ActivityGUID;
                        ActivityEntity nextActivity = service.GetNextActivity(processGUID, firstActivityGUID);
                        GetStepList(nextActivity.ActivityGUID.ToString(), nextActivity.ActivityName, nextActivity.Description);
                        GetuserList(nextActivity.Roles);
                        break;
                    case "task":
                        int instanceId = Request.QueryString["instanceId"] == null ? 0 : Convert.ToInt32(Request.QueryString["instanceId"].ToString());
                        WfAppRunner runner = new WfAppRunner();
                        runner.AppInstanceID = instanceId;
                        runner.ProcessGUID = processGUID;
                        runner.UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId();

                        IList<NodeView> NodeViewList = service.GetNextActivityTree(runner, null);
                        NodeView nodeView = NodeViewList[0];

                        var runningNode = service.GetRunningNode(runner);
                        Guid onActivityGUID = runningNode.ActivityGUID;
                        ActivityEntity ActivityStep = service.GetNextActivity(processGUID, onActivityGUID);
                        if (ActivityStep.ActivityType == ActivityTypeEnum.EndNode)
                        {
                        }
                        //
                        GetStepList(ActivityStep.ActivityGUID.ToString(), ActivityStep.ActivityName, ActivityStep.Description);
                        GetuserList(ActivityStep.Roles);
                        break;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <param name="activityName"></param>
        /// <param name="activityDescription"></param>
        protected void GetStepList(string activityGUID, string activityName, string activityDescription)
        {
            StringBuilder sb = new StringBuilder("");
            if (!string.IsNullOrEmpty(activityDescription))
            {
                activityDescription = "-" + activityDescription;
            }
            sb.Append("<input name=\"radioStep\" id=\"radioStep_" + activityGUID + "\" type=\"radio\" value=\"" + activityGUID + "\" />" + activityName + activityDescription + "<br />");

            this.stepList = sb.ToString();
        }

        /// <summary>
        /// 得到人员列表
        /// </summary>
        /// <param name="roleList"></param>
        protected void GetuserList(IList<int> roleList)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder("");
            foreach (int id in roleList)
            {
                if (i > 0)
                    sb.Append(",");
                sb.Append(id);
                i++;
            }
            string roleIdList = sb.ToString();
            if (!string.IsNullOrEmpty(roleIdList))
            {
                DataTable dt = Wf5.WebDemo.Business.WorkFlows.GetUserListByRoleIdList(roleIdList);
                StringBuilder sbUser = new StringBuilder("");
                foreach (DataRow dr in dt.Rows)
                {
                    sbUser.Append("<input name=\"radioUser\" id=\"radioUser_" + dr["uID"] + "\" type=\"radio\" value=\"" + dr["uID"] + "\" />" + dr["uName"] + " - " + dr["rName"] + "<br />");
                }
                userList = sbUser.ToString();
            }
        }
    }
}