using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Xpdl;

namespace Wf5.Engine.Core.Pattern
{
    /// <summary>
    /// 路由接口
    /// </summary>
    internal interface ICompleteAutomaticlly
    {
        GatewayExecutedResult CompleteAutomaticlly(ProcessInstanceEntity processInstance,
            Guid transitionGUID,
            ActivityInstanceEntity fromActivityInstance,
            ActivityResource activityResource,
            IDbSession session);
    }
}
