using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Wf5.WebDemo
{
    public partial class FlowOpinion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["Action"] != null && Request.QueryString["Action"].ToString() == "ShowOpinion")
                {
                    InitOpinion();
                }
            }
        }

        protected void InitOpinion()
        {
            int instanceId = Request.QueryString["instanceId"] == null ? 0 : Convert.ToInt32(Request.QueryString["instanceId"].ToString());
            if (instanceId > 0)
            {
                DataTable dt = Wf5.WebDemo.Business.WorkFlows.GetFlowOpinionList(instanceId);
                dt.Columns.Add("StateName", typeof(string)).DefaultValue = "";
                foreach (DataRow dr in dt.Rows)
                {
                    switch (Convert.ToInt32(dr["ActivityState"].ToString()))
                    {
                        case 1:
                            dr["StateName"] = "准备状态";
                            break;

                        case 2:
                            dr["StateName"] = "运行状态";
                            break;


                        case 4:
                            dr["StateName"] = "完成状态";
                            break;


                        case 8:
                            dr["StateName"] = "撤销状态";
                            break;

                        case 16:
                            dr["StateName"] = "退回状态";
                            break;
                    }
                }
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }
        }

    }
}