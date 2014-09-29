using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Common;
using Wf5.Engine.Service;

namespace Wf5.BizApp
{
    /// <summary>
    /// 用户部门关系管理
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        /// <summary>
        /// 获取任务办理人列表
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public PerformerList GetPerformerList(int processInstanceID, List<int> roles)
        {
            PerformerList performers = null;

            //获取流程发起人信息，在xml定义中有role变量：-1
            if (roles.Count == 1 && roles[0] == -1)
            {
                IWorkflowService service = new WorkflowService();
                Performer initiator = service.GetProcessInitiator(processInstanceID);
                performers = new PerformerList();
                performers.Add(initiator);
            } 

            return performers;
        }
    }
}
