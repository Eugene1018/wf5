using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Xpdl.Node
{
    /// <summary>
    /// 互斥合并类型的节点
    /// </summary>
    internal class XOrJoinNode : NodeBase
    {
        internal XOrJoinNode(ActivityEntity activity)
            : base(activity)
        {

        }
    }
}
