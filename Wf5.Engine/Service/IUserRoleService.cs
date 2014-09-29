using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wf5.Engine.Common;

namespace Wf5.Engine.Service
{
    /// <summary>
    /// 用户部门关系管理服务接口
    /// </summary>
    public interface IUserRoleService
    {
        PerformerList GetPerformerList(int processInstanceID, List<int> roles);
    }
}
