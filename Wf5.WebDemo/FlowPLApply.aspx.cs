using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Common;
using Wf5.Engine.Service;

namespace Wf5.WebDemo
{
    public partial class FlowPLApply : System.Web.UI.Page
    {
        IWorkflowService wfService = new WorkflowService();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["proid"]))
                {
                    ProcessInstanceEntity proModel = Wf5.WebDemo.Data.WorkFlowManager.GetProcessInstance(int.Parse(Request.QueryString["proid"]));
                    txtApplyTitle.Value = proModel.AppName;
                    hidProId.Value = proModel.ProcessInstanceID.ToString();
                }
                if (!string.IsNullOrEmpty(Request.QueryString["taskid"]))
                {
                    taskid.Value = Request.QueryString["taskid"];
                    ReceiveTask();
                    
                }
            }
        }

        private void ReceiveTask()
        {
            
            var initiator = new WfAppRunner();
            initiator.UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId();
            initiator.UserName = HttpContext.Current.Session["UserName"].ToString();
            initiator.TaskID = int.Parse(Request.QueryString["taskid"]);
            bool isRead = wfService.SetTaskRead(initiator);

        }

        private WfAppRunner GetRunner()
        {
            var initiator = new WfAppRunner();
            initiator.AppName = txtApplyTitle.Value;
            initiator.AppInstanceID = 110;
            initiator.ProcessGUID = Guid.Parse(selectSplitFlow.Value);
            initiator.UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId();
            initiator.UserName = HttpContext.Current.Session["UserName"].ToString();


            return initiator;
        }


        //退回至上一步节点
        //{"UserID":"10","UserName":"Long","AppName":"SamplePrice","AppInstanceID":"100","ProcessGUID":"072af8c3-482a-4b1c-890b-685ce2fcc75d"}
        protected void SendBack_Click(object sender, EventArgs e)
        {
            if (Wf5.WebDemo.Data.WorkFlowManager.SendBack(GetRunner()))
            {
                Response.Redirect("FlowList.aspx?flag=flowTodo");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            var runner = GetRunner();

            bool flag = wfService.CancelProcess(runner);
            if (flag)
                Response.Redirect("FlowList.aspx?flag=flowTodo");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Response.Redirect("NextStep.aspx?days=" + txtDays.Text + "&roleid=" + roleid.Value + "&taskid=" + taskid.Value+"&appname="+txtApplyTitle.Value+"&proid="+hidProId.Value);
        }
    }
}