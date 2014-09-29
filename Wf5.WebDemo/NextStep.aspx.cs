using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wf5.Engine.Common;
using Wf5.Engine.Service;

namespace Wf5.WebDemo
{
    public partial class NextStep : System.Web.UI.Page
    {
        IWorkflowService wfService = new WorkflowService();
        IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Wf5ConnectionString"].ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //根据条件取得下一步骤
                Dictionary<string, string> condition = new Dictionary<string, string>();
                condition.Add("roleid", Wf5.WebDemo.Data.WorkFlowManager.GetFirstStepRole(Request.QueryString["proid"]));
                condition.Add("days", Request.QueryString["days"]);
                IUserRoleService roleService = new Wf5.BizApp.UserRoleService();
                IList<NodeView> nextNodes = wfService.GetNextActivityTree(int.Parse(Request.QueryString["taskid"]), condition);//int.Parse(Request.QueryString["taskid"])
                if (nextNodes != null)
                {
                    Repeater1.DataSource = nextNodes;
                    Repeater1.DataBind();

                }
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            submitWork();
        }

        private void submitWork()
        {
            //提交
            var initiator = GetRunner();
            Dictionary<string, string> condition = new Dictionary<string, string>();
            condition.Add("roleid", Wf5.WebDemo.Data.WorkFlowManager.GetFirstStepRole(Request.QueryString["proid"]));
            condition.Add("days", Request.QueryString["days"]);
            IList<NodeView> nextNodes = wfService.GetNextActivityTree(int.Parse(Request.QueryString["taskid"]), condition);
            //下一步接收人
            var banFangNodeGuid = nextNodes[nextNodes.Count - 1].ID;
            PerformerList pList = new PerformerList();
            for (int i = 0; i < nextNodes[nextNodes.Count - 1].Roles.Count; i++)
            {
                pList.Add(new Performer(nextNodes[nextNodes.Count - 1].Roles[i],
                    Wf5.WebDemo.Business.WorkFlows.GetUserListByRoleIdList(nextNodes[nextNodes.Count - 1].Roles[i].ToString()).Rows[0]["uname"].ToString()));
            }
            initiator.NextActivityPerformers = new Dictionary<Guid, PerformerList>();
            initiator.NextActivityPerformers.Add(banFangNodeGuid, pList);

            //流程开始->业务员提交
            conn.Open();
            IDbTransaction trans = conn.BeginTransaction();

            try
            {
                var result = wfService.RunProcessApp(conn, initiator, trans);

                trans.Commit();
                Response.Redirect("FlowList.aspx?flag=flowTodo");
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private WfAppRunner GetRunner()
        {
            var initiator = new WfAppRunner();
            initiator.AppName = Request.QueryString["appname"];
            initiator.AppInstanceID = 110;

            initiator.ProcessGUID = Guid.Parse(selectSplitFlow.Value);
            initiator.Conditions = new Dictionary<string, string>();
            initiator.Conditions.Add("roleid", Wf5.WebDemo.Data.WorkFlowManager.GetFirstStepRole(Request.QueryString["proid"]));
            initiator.Conditions.Add("days", Request.QueryString["days"]);

            initiator.UserID = Wf5.WebDemo.Common.Helper.GetLoginUserId();
            initiator.UserName = HttpContext.Current.Session["UserName"].ToString();


            return initiator;
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rep = e.Item.FindControl("Repeater2") as Repeater;//找到里层的repeater对象
                NodeView rowv = (NodeView)e.Item.DataItem;//找到分类Repeater关联的数据项 
                if (rowv.Roles != null)
                {
                    DataTable dt = GetuserList(rowv.Roles);
                    if (dt != null)
                    {
                        hidUserCount.Value = dt.Rows.Count.ToString();
                        rep.DataSource = dt;
                        rep.DataBind();
                    }
                }

            }
        }

        /// <summary>
        /// 得到人员列表
        /// </summary>
        /// <param name="roleList"></param>
        protected DataTable GetuserList(IList<int> roleList)
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
                return dt;
            }
            return null;
        }
    }
}