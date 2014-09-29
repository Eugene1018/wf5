using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Wf5.WebDemo.Entity;
using Wf5.WebDemo.Business;
using Wf5.WebDemo.Common;

namespace Wf5.WebDemo
{
    public partial class FlowApproval : System.Web.UI.Page
    {
        public string instanceId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["Action"] != null)
                {
                    string Action = Request.QueryString["Action"].ToString();
                    switch (Action)
                    {
                        case "FlowApproval":
                        case "FlowReadOnly":
                            InitFlowApproval(Action);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitFlowApproval(string Action)
        {
            int _instanceId = Request.QueryString["instanceId"] == null ? 0 : Convert.ToInt32(Request.QueryString["instanceId"].ToString());
            instanceId = _instanceId.ToString();
            FlowInstanceInfo wfInfo = WorkFlows.GetFlowInstanceModel(_instanceId);
            if (wfInfo != null)
            {
                int objectId = wfInfo.fiObjectId;
                this.spanNo.InnerText = wfInfo.fiNo;
                hiddenInstanceId.Value = wfInfo.fiID.ToString();
                hiddenObjectId.Value = wfInfo.fiObjectId.ToString();
                flowGuid.Value = wfInfo.fiFlowDefinitionGuid.ToString();
                string level = wfInfo.fiLevel;
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
                this.spanLevel.InnerText = levelName;

                this.spanTitle.InnerText = wfInfo.fiTitle;
                this.spanState.InnerText = "办理中";

                UserInfo userInfo = WorkFlows.GetUserModel(Convert.ToInt32(wfInfo.fiApplyer));
                if (userInfo != null)
                    this.spanApplyer.InnerText = userInfo.uName;

                this.spanApplyTime.InnerText = wfInfo.fiApplyTime.ToString("yyyy-MM-dd HH:mm ss");
                this.spanApplyReason.InnerText = wfInfo.fiApplyReason;
                if (Action == "FlowReadOnly")
                {
                    form_control_1.Visible = false;
                    form_control_2.Visible = true;
                }
                else
                {
                    form_control_1.Visible = true;
                    form_control_2.Visible = false;
                }
            }
        }
    }
}