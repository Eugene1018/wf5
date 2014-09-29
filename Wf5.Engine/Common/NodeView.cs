using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Business;

namespace Wf5.Engine.Common
{
    /// <summary>
    /// 工作流流转节点的视图对象
    /// </summary>
    public class NodeView
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public IList<int> Roles { get; set; }
        public PerformerList PerformerList { get; set; }
    }
}
