using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;

using Wf5.WebDemo.Common;
using Wf5.WebDemo.Entity;
using Wf5.Engine.Common;
using Wf5.Engine.Service;
using Wf5.Engine.Business.Entity;


namespace Wf5.WebDemo.Data
{
    public class WorkFlowManager
    {
        /// <summary>
        /// 获取系统中的角色列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRoleList()
        {
            string strSql = "select * from Wf_Demo_Role where 1=1";
            return SQLHelper.ExecuteDataset(strSql).Tables[0];
        }

        /// <summary>
        /// 根据角色ID查询用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static DataTable GetUserListByRoleId(int roleId)
        {
            string strSql = "select * from Wf_Demo_User where 1=1 and uRoleId='" + roleId + "'";
            return SQLHelper.ExecuteDataset(strSql).Tables[0];
        }

        /// <summary>
        /// 根据角色ID查询用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static DataTable GetUserListByRoleIdList(string roleIdList)
        {
            string strSql = "select  u.*,r.* from Wf_Demo_User u, dbo.Wf_Demo_Role r where 1=1 and u.uRoleId=r.rId and uRoleId in(" + roleIdList + ") ";
            return SQLHelper.ExecuteDataset(strSql).Tables[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public static Wf5.WebDemo.Entity.UserInfo GetUserModel(int uId)
        {
            string strSql = "select * from Wf_Demo_User where 1=1 and uId='" + uId + "'";
            DataTable dt = SQLHelper.ExecuteDataset(strSql).Tables[0];
            if (dt != null && dt.Rows != null && dt.Rows.Count == 1)
            {
                Wf5.WebDemo.Entity.UserInfo model = new Entity.UserInfo();
                if (dt.Rows[0]["uID"] != null)
                {
                    model.uID = Convert.ToInt32(dt.Rows[0]["uID"].ToString());
                }
                else
                {
                    model.uID = 0;
                }
                if (dt.Rows[0]["uRoleId"] != null)
                {
                    model.uRoleId = Convert.ToInt32(dt.Rows[0]["uRoleId"].ToString());
                }
                else
                {
                    model.uRoleId = 0;
                }

                model.uName = dt.Rows[0]["uName"] == null ? "" : dt.Rows[0]["uName"].ToString();

                return model;
            }
            return null;
        }



        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static FlowInstanceInfo GetFlowInstanceModel(int fiID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select fiID, fiFlowDefinitionGuid, fiType, fiObjectId, fiSerialNo, fiNo, fiState, fiLevel, fiTitle, fiApplyReason, fiApplyer, fiApplyTime, fiLastOperator, fiLastOptTime  ");
            strSql.Append("  from Wf_Demo_FlowInstance ");
            strSql.Append(" where fiID=@fiID");
            SqlParameter[] parameters = {
					new SqlParameter("@fiID", SqlDbType.Int,4)
			};
            parameters[0].Value = fiID;

            FlowInstanceInfo model = new FlowInstanceInfo();
            DataSet ds = SQLHelper.ExecuteDataset(strSql.ToString(), parameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["fiID"].ToString() != "")
                {
                    model.fiID = int.Parse(ds.Tables[0].Rows[0]["fiID"].ToString());
                }
                if (ds.Tables[0].Rows[0]["fiFlowDefinitionGuid"].ToString() != "")
                {
                    model.fiFlowDefinitionGuid = Guid.Parse(ds.Tables[0].Rows[0]["fiFlowDefinitionGuid"].ToString());
                }
                model.fiType = ds.Tables[0].Rows[0]["fiType"].ToString();
                if (ds.Tables[0].Rows[0]["fiObjectId"].ToString() != "")
                {
                    model.fiObjectId = int.Parse(ds.Tables[0].Rows[0]["fiObjectId"].ToString());
                }
                model.fiSerialNo = ds.Tables[0].Rows[0]["fiSerialNo"].ToString();
                model.fiNo = ds.Tables[0].Rows[0]["fiNo"].ToString();
                model.fiState = ds.Tables[0].Rows[0]["fiState"].ToString();
                model.fiLevel = ds.Tables[0].Rows[0]["fiLevel"].ToString();
                model.fiTitle = ds.Tables[0].Rows[0]["fiTitle"].ToString();
                model.fiApplyReason = ds.Tables[0].Rows[0]["fiApplyReason"].ToString();
                model.fiApplyer = ds.Tables[0].Rows[0]["fiApplyer"].ToString();
                if (ds.Tables[0].Rows[0]["fiApplyTime"].ToString() != "")
                {
                    model.fiApplyTime = DateTime.Parse(ds.Tables[0].Rows[0]["fiApplyTime"].ToString());
                }
                model.fiLastOperator = ds.Tables[0].Rows[0]["fiLastOperator"].ToString();
                if (ds.Tables[0].Rows[0]["fiLastOptTime"].ToString() != "")
                {
                    model.fiLastOptTime = DateTime.Parse(ds.Tables[0].Rows[0]["fiLastOptTime"].ToString());
                }

                return model;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取当前用户待办任务列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetReadTasks(int userId)
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(" SELECT t.*,                                                     ");
            sb.Append("       fi.*                                           ");
            sb.Append(" FROM   dbo.vwWfActivityInstanceTasks t                          ");
            sb.AppendFormat("       inner join Wf_Demo_FlowInstance fi           ");
            sb.Append("         on fi.fiFlowDefinitionGuid = t.ProcessGUID             ");
            sb.Append("            and fi.fiID = t.AppInstanceID                       ");
            // sb.AppendFormat("    left join  Wf_Demo_FlowDefinition fd");
            // sb.Append(" on fi.fiFlowDefinitionGuid = fd.fdGuid");
            //sb.AppendFormat(" left join Wf_Demo_FlowType ft");
            // sb.Append(" on ft.ftID = fd.fdTypeID");
            sb.Append(" WHERE  1=1                                                      ");
            sb.AppendFormat("       AND AssignedToUserID ={0}", userId);
            sb.Append("       AND ActivityState = 1                                    ");
            sb.Append("       AND ProcessState = 2                                     ");
            sb.Append("       AND ActivityType = 4                                     ");
            sb.Append("                                                                ");

            return SQLHelper.ExecuteDataset(sb.ToString()).Tables[0];
        }



        /// <summary>
        /// 批量保存流程记录数据
        /// </summary>
        /// <param name="flowRecordInfoList"></param>
        /// <returns></returns>
        public static bool SaveFlowFlowRecord(List<Wf5.WebDemo.Entity.FlowRecordInfo> flowRecordInfoList)
        {
            List<CommandInfo> commandInfoList = new List<CommandInfo>();
            CommandInfo commandInfo;
            foreach (Wf5.WebDemo.Entity.FlowRecordInfo model in flowRecordInfoList)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat("insert into Wf_Demo_FlowRecord(");
                strSql.Append(" frProcessGuid,frProcessInstanceID,frAppInstanceID,frActivityInstanceID,frActivityGuid,frActivityType,frActivityState,frDealAdvice,frFile,frSignInfo,frOperator,frOperatorTime");
                strSql.Append(" ) values (");
                strSql.Append(" @frProcessGuid,@frProcessInstanceID,@frAppInstanceID,@frActivityInstanceID,@frActivityGuid,@frActivityType,@frActivityState,@frDealAdvice,@frFile,@frSignInfo,@frOperator,getdate()");
                strSql.Append(" ) ");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
			            new SqlParameter("@frProcessGuid", SqlDbType.UniqueIdentifier,16) ,            
                        new SqlParameter("@frProcessInstanceID", SqlDbType.Int,4) ,            
                        new SqlParameter("@frAppInstanceID", SqlDbType.Int,4) ,            
                        new SqlParameter("@frActivityInstanceID", SqlDbType.Int,4) ,            
                        new SqlParameter("@frActivityGuid", SqlDbType.UniqueIdentifier,16) ,            
                        new SqlParameter("@frActivityType", SqlDbType.Int,4) ,            
                        new SqlParameter("@frActivityState", SqlDbType.Int,4) ,            
                        new SqlParameter("@frDealAdvice", SqlDbType.VarChar,500) ,            
                        new SqlParameter("@frFile", SqlDbType.VarChar,150) ,            
                        new SqlParameter("@frSignInfo", SqlDbType.VarChar,500) ,            
                        new SqlParameter("@frOperator", SqlDbType.Int,4) //,            
                };
                int idx = 0;

                parameters[idx++].Value = model.frProcessGuid;
                parameters[idx++].Value = model.frProcessInstanceID;
                parameters[idx++].Value = model.frAppInstanceID;
                parameters[idx++].Value = model.frActivityInstanceID;
                parameters[idx++].Value = model.frActivityGuid;
                parameters[idx++].Value = model.frActivityType;
                parameters[idx++].Value = model.frActivityState;
                parameters[idx++].Value = model.frDealAdvice;
                parameters[idx++].Value = model.frFile;
                parameters[idx++].Value = model.frSignInfo;
                parameters[idx++].Value = model.frOperator;

                commandInfo = new CommandInfo();
                commandInfo.CommandText = strSql.ToString();
                commandInfo.Parameters = parameters;
                commandInfoList.Add(commandInfo);

            }
            int flag = SQLHelper.ExecuteSqlTran(commandInfoList);
            return flag > 0;
        }

        /// <summary>
        /// 业务流程申请【业务流程申请第一步数据保存】
        /// </summary>
        /// <param name="FlowInstanceInfo">流程实例信息</param>
        /// <returns></returns>
        public static int FlowApply(Wf5.WebDemo.Entity.FlowInstanceInfo flowInstanceInfo)
        {
            try
            {
                //向流程实例表插入记录
                StringBuilder strSqlInstance = new StringBuilder();
                strSqlInstance.Append("insert into Wf_Demo_FlowInstance(");
                strSqlInstance.Append("fiFlowDefinitionGuid,fiType,fiObjectId,fiSerialNo,fiNo,fiState,fiLevel,fiTitle,fiApplyReason,fiApplyer,fiApplyTime,fiLastOperator,fiLastOptTime");
                strSqlInstance.Append(") values (");
                strSqlInstance.Append("@fiFlowDefinitionGuid,@fiType,@fiObjectId,@fiSerialNo,@fiNo,@fiState,@fiLevel,@fiTitle,@fiApplyReason,@fiApplyer,getdate(),@fiLastOperator,getdate()");
                strSqlInstance.Append(") ");
                strSqlInstance.Append(";select @@IDENTITY");
                SqlParameter[] parametersInstance = {
			            new SqlParameter("@fiFlowDefinitionGuid", SqlDbType.UniqueIdentifier,16) ,            
                        new SqlParameter("@fiType", SqlDbType.Char,1) ,            
                        new SqlParameter("@fiObjectId", SqlDbType.Int,4) ,            
                        new SqlParameter("@fiSerialNo", SqlDbType.VarChar,100) ,            
                        new SqlParameter("@fiNo", SqlDbType.VarChar,50) ,            
                        new SqlParameter("@fiState", SqlDbType.Char,1) ,            
                        new SqlParameter("@fiLevel", SqlDbType.Char,1) ,            
                        new SqlParameter("@fiTitle", SqlDbType.VarChar,100) ,            
                        new SqlParameter("@fiApplyReason", SqlDbType.VarChar,150) ,            
                        new SqlParameter("@fiApplyer", SqlDbType.VarChar,20) ,            
                        new SqlParameter("@fiLastOperator", SqlDbType.VarChar,20) 
                        //new SqlParameter("@ReturnValue", SqlDbType.Int,4)               
                        };

                int idxInstance = 0;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiFlowDefinitionGuid;// Guid.NewGuid();
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiType;// model.fiType;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiObjectId;//model.fiFormID
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiSerialNo;// model.fiSerialNo;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiNo;// model.fiNo;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiState;// model.fiState; 
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiLevel;// model.fiLevel;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiTitle;// model.fiTitle;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiApplyReason == null ? "" : flowInstanceInfo.fiApplyReason;//model.fiApplyAReason;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiApplyer;//model.fiApplyer;
                parametersInstance[idxInstance++].Value = flowInstanceInfo.fiLastOperator == null ? "" : flowInstanceInfo.fiLastOperator;// model.fiLastOperator;

                object obj = SQLHelper.ExecuteScalar(strSqlInstance.ToString(), parametersInstance);
                if (obj != null)
                {
                    return Convert.ToInt32(obj.ToString());
                }
                return -1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 获得办理意见集合
        /// </summary>
        /// <param name="instanceId"></param>
        public static DataTable GetFlowOpinionList(int instanceId)
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append(" SELECT ai.*, ");
            sb.Append("       fr.* ");
            sb.Append(" FROM   WfActivityInstance ai ");
            sb.Append("       LEFT JOIN  Wf_Demo_FlowRecord fr ");
            sb.Append("         ON ai.ActivityInstanceID = fr.frActivityInstanceID ");
            sb.Append(" WHERE  1 = 1 ");
            sb.Append("       AND ai.ProcessInstanceID = fr.frProcessInstanceID ");
            sb.Append("       AND ai.AppInstanceID = fr.frAppInstanceID ");
            sb.Append("       AND AI.AppInstanceID = " + instanceId + " ");
            return SQLHelper.ExecuteDataset(sb.ToString()).Tables[0];
        }


        //流程启动
        public static void ProcessStart(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(SQLHelper.m_StrConn);
            //流程启动
            conn.Open();
            IDbTransaction trans = conn.BeginTransaction();

            try
            {
                Engine.Core.Result.WfStartedResult re = wfService.StartProcess(conn, runner, trans);
                
                trans.Commit();

            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                trans.Dispose();
            }
        }

        public static ProcessInstanceEntity GetProcessInstance(int proId)
        {
            IWorkflowService wfService = new WorkflowService();
            return wfService.GetProcessInstance(proId);
        }
        //回退
        public static bool SendBack(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(SQLHelper.m_StrConn);
            bool flag = false;
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.SendBackProcess(conn,runner, trans);
                trans.Commit();
                flag = true;
            }
            catch (WorkflowException w)
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
            return flag;
        }
        //撤销
        public static void WithdrawProcess(WfAppRunner runner)
        {
            IWorkflowService wfService = new WorkflowService();
            IDbConnection conn = new SqlConnection(SQLHelper.m_StrConn);
            conn.Open();

            IDbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction();
                var result = wfService.WithdrawProcess(conn, runner, trans);
                trans.Commit();
            }
            catch (WorkflowException w)
            {
                trans.Rollback();
            }
            finally
            {
                trans.Dispose();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        //得到发起者角色
        public static string GetFirstStepRole(string proId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select uRoleId from Wf_Demo_User ");
            sb.Append(" where uID=(select top 1 AssignedToUserID from vwWfActivityInstanceTasks where ProcessInstanceID="+proId+" order by TaskID)");

            DataSet ds = SQLHelper.ExecuteDataset(sb.ToString());
            if(ds !=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }
        //得到发起者角色
        public static string GetProId(string proGUID,string appName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select top 1 ProcessInstanceID from WfProcessInstance ");
            sb.Append(" where ProcessGUID='" + proGUID + "' and appname='"+appName+"' order by ProcessInstanceID desc");

            DataSet ds = SQLHelper.ExecuteDataset(sb.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }
    }
}