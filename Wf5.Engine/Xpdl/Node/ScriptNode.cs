using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Wf5.Engine.Common;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Xpdl.Node
{
    internal class ScriptNode : NodeBase, IDynamicRunable
    {
        internal ScriptNode(ActivityEntity activity)
            : base(activity)
        {

        }

        #region IDynamicRunable Members
        public object InvokeMethod(TaskImplementDetail implementation, object[] userParameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
