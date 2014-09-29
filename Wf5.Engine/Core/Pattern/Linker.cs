using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Wf5.Engine.Data;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Xpdl;
using Wf5.Engine.Xpdl.Node;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Core.Result;

namespace Wf5.Engine.Core.Pattern
{
    /// <summary>
    /// 节点连接线
    /// </summary>
    public class Linker
    {
        /// <summary>
        /// 起始节点定义
        /// </summary>
        public ActivityEntity FromActivity { get; set; }
        
        /// <summary>
        /// 起始节点实例
        /// </summary>
        public ActivityInstanceEntity FromActivityInstance { get; set; }

        /// <summary>
        /// 到达节点定义
        /// </summary>
        public ActivityEntity ToActivity { get; set; }

        /// <summary>
        /// 到达节点实例
        /// </summary>
        public ActivityInstanceEntity ToActivityInstance { get; set; } 
    }
}
