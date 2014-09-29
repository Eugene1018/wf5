using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Xpdl.Node
{
    /// <summary>
    /// 与分支节点
    /// </summary>
    internal class AndSplitNode : NodeBase
    {
        internal AndSplitNode(ActivityEntity activity)
            : base(activity)
        {
        }
    }
}
