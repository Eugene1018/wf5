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
    /// 子流程节点
    /// </summary>
    internal class SubProcessNode : NodeBase
    {
        public Nullable<Guid> SubProcessGUID { get; set; }

        internal SubProcessNode(ActivityEntity activity) :
            base(activity)
        {

        }
    }
}
