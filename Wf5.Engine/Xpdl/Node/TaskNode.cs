﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Wf5.Engine.Common;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Xpdl.Node
{
    /// <summary>
    /// 任务节点
    /// </summary>
    internal class TaskNode : NodeBase
    {
        internal TaskNode(ActivityEntity activity) :
            base(activity)
        {

        }
    }
}
