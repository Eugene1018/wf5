using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Wf5.WebDemo.Common;
using Wf5.Engine.Service;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Common;

namespace Wf5.WebDemo
{
    public partial class FlowList : System.Web.UI.Page
    {
        public string flowInstance = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string loginUserName = Helper.GetLoginUserName();
                if (string.IsNullOrEmpty(loginUserName))
                    lLoginInfo.Text = "未登录";
                else
                {
                    lLoginInfo.Text = string.Format("当前登录角色：{0}   当前登录者：{1}", Helper.GetLoginRoleName(), Helper.GetLoginUserName());
                }
                if (Request.QueryString["type"] == "withdraw")
                {
                    WithdrawProcess();
                }
                InitList();
                GetRunningWork();
                GetFlowApprovalList();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected void GetFlowApprovalList()
        {
            string flag = Request.QueryString["flag"] == null ? "" : Request.QueryString["flag"].ToString();
            if (string.IsNullOrEmpty(flag))
                flag = "flowTodo";
            flowInstance = Wf5.WebDemo.Business.WorkFlows.GetReadTasksDataList(flag);
        }


        private void InitList()
        {
            IWorkflowService wfService = new WorkflowService();
            TaskQueryEntity en = new TaskQueryEntity
            {
                UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId()

            };
            IList<TaskViewEntity> lstWait = wfService.GetReadyTasks(en);
            if (lstWait != null)
            {
                Repeater1.DataSource = lstWait;
                Repeater1.DataBind();
            }
        }

        private void GetRunningWork()
        {
            IWorkflowService wfService = new WorkflowService();
            TaskQueryEntity en = new TaskQueryEntity
            {
                UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId()


            };
            IList<TaskViewEntity> lstWait = wfService.GetRunningTasks(en);
            if (lstWait != null)
            {
                Repeater2.DataSource = lstWait;
                Repeater2.DataBind();
            }
        }

        public void WithdrawProcess()
        {
            Wf5.WebDemo.Data.WorkFlowManager.WithdrawProcess(GetRunner());
        }

        private WfAppRunner GetRunner()
        {
            var initiator = new WfAppRunner();
            initiator.AppName = Request.QueryString["appname"];
            initiator.AppInstanceID = 110;
            initiator.ProcessGUID = Guid.Parse(Request.QueryString["proid"]);
            initiator.UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId();
            initiator.UserName = HttpContext.Current.Session["UserName"].ToString();

            return initiator;
        }
    }
}