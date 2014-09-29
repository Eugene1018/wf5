using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wf5.Engine.Common;
using Wf5.WebDemo.Entity;

namespace Wf5.WebDemo
{
    public partial class FlowPLApplyStart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            //流程启动
            Wf5.WebDemo.Data.WorkFlowManager.ProcessStart(GetRunner());

            Response.Redirect("FlowPLApply.aspx?proid=" + Wf5.WebDemo.Data.WorkFlowManager.GetProId(selectSplitFlow.Value, txtApplyTitle.Value));
        }
    }
}