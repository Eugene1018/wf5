using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wf5.WebDemo
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Remove("UserId");
                HttpContext.Current.Session.Remove("UserName");
                HttpContext.Current.Session.Remove("RoleId");

                Wf5.WebDemo.Common.Helper.BindDropDownList(this.ddlRole, Wf5.WebDemo.Business.WorkFlows.GetRoleList(), "rName", "rId", true, "请选择角色", "0");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["UserId"] = this.ddlUser.SelectedValue.ToString();
            HttpContext.Current.Session["UserName"] = this.ddlUser.SelectedItem.Text.ToString();
            HttpContext.Current.Session["RoleId"] = this.ddlRole.SelectedValue.ToString();
            HttpContext.Current.Session["RoleName"] = this.ddlRole.SelectedItem.Text.ToString();
            HttpContext.Current.Session.Timeout = 60;
            HttpContext.Current.Response.Redirect("FlowList.aspx?flag=flowTodo");
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rId = Convert.ToInt32(ddlRole.SelectedValue.ToString());
            Wf5.WebDemo.Common.Helper.BindDropDownList(this.ddlUser, Wf5.WebDemo.Business.WorkFlows.GetUserListByRoleId(rId), "uName", "uID");
        }
    }
}