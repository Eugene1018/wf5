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
    /// 异或类型的节点
    /// </summary>
    internal class XOrSplitNode : NodeBase
    {
        internal XOrSplitNode(ActivityEntity activity)
            : base(activity)
        {
        }
        
    }
}
