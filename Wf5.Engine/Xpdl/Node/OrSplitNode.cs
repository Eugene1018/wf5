using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Data;
using Wf5.Engine.Common;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Xpdl.Node
{
    /// <summary>
    /// 或分支节点
    /// </summary>
    internal class OrSplitNode : NodeBase
    {
        internal OrSplitNode(ActivityEntity activity) :
            base(activity)
        {
        }
        
    }
}
