using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Common;

namespace Wf5.Engine.Xpdl.Node
{
    /// <summary>
    /// 开始节点
    /// </summary>
    internal class StartNode : NodeBase
    {
        internal StartNode(ActivityEntity activity)
            : base(activity)
        {

        }
    }
}
