using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using Wf5.WebDemo.Entity;

namespace Wf5.WebDemo.Handler
{
    /// <summary>
    /// FlowApply 的摘要说明
    /// </summary>
    public class FlowApply : IHttpHandler, IRequiresSessionState 
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string rtMsg = string.Empty;
            int nextActivityUserId = 0;
            string nextActivityGuid = string.Empty;
            string refMessage = string.Empty;
            try
            {
                if (context.Request.Form["Action"] != null)
                {
                    string Action = context.Request.Form["Action"].ToString();
                    switch (Action)
                    {
                        case "FlowApply": 
                            string flowInstanceInfo = string.Empty;

                            if (context.Request.Form["flowInstanceInfo"] != null)
                            {
                                flowInstanceInfo = context.Request.Form["flowInstanceInfo"].ToString();
                            }

                            if (context.Request.Form["nextActivityGuid"] != null)
                            {
                                nextActivityGuid = context.Request.Form["nextActivityGuid"].ToString();
                            }

                            if (context.Request.Form["nextActivityUserId"] != null)
                            {
                                nextActivityUserId = Convert.ToInt32(context.Request.Form["nextActivityUserId"].ToString());
                            }
                            FlowInstanceInfo wfFlowInstanceInfo = Wf5.WebDemo.Common.Helper.JsonToObject<FlowInstanceInfo>(flowInstanceInfo);
                            bool result = Wf5.WebDemo.Business.WorkFlows.FlowApply(nextActivityGuid, nextActivityUserId, wfFlowInstanceInfo, ref refMessage);
                            if (result)
                            {
                                rtMsg = "success";
                            }
                            else
                            {
                                rtMsg = "failure";
                            }
                            rtMsg = Wf5.WebDemo.Common.Helper.ObjectToJson(rtMsg);
                            break;

                        case "SaveFlowApproval":
                            int instanceId = 0;
                            string approvalMemo = string.Empty;

                            if (context.Request.Form["instanceId"] != null)
                            {
                                instanceId = context.Request.Form["instanceId"].ToString() == null ? 0 : Convert.ToInt32(context.Request.Form["instanceId"].ToString());
                            }

                            if (context.Request.Form["nextActivityGuid"] != null)
                            {
                                nextActivityGuid = context.Request.Form["nextActivityGuid"].ToString();
                            }

                            if (context.Request.Form["nextActivityUserId"] != null)
                            {
                                nextActivityUserId = context.Request.Form["nextActivityUserId"].ToString() == null ? 0 : Convert.ToInt32(context.Request.Form["nextActivityUserId"].ToString());
                            }

                            if (context.Request.Form["approvalMemo"] != null)
                            {
                                approvalMemo = context.Request.Form["approvalMemo"].ToString();
                            }

                            result = Wf5.WebDemo.Business.WorkFlows.FlowApproval(instanceId, nextActivityGuid, nextActivityUserId, approvalMemo, ref refMessage);
                            if (result)
                            {
                                rtMsg = "success";
                            }
                            else
                            {
                                rtMsg = "failure";
                            }
                            rtMsg = Wf5.WebDemo.Common.Helper.ObjectToJson(rtMsg);
                            break;
                    }
                }
                context.Response.Write(rtMsg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}