﻿using System;
using System.Threading;
using System.Data.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Xpdl;


namespace Wf5.Engine.Core
{
    /// <summary>
    /// 流程回退处理时的上下文对象
    /// </summary>
    internal class BackwardContext
    {
        internal ActivityEntity BackwardToTaskActivity { get; set; }
        internal ActivityInstanceEntity BackwardToTaskActivityInstance { get; set; }
        internal ActivityEntity BackwardFromActivity { get; set; }
        internal ActivityInstanceEntity BackwardFromActivityInstance { get; set; }
        internal ProcessInstanceEntity ProcessInstance { get; set; }
        internal Guid BackwardToTargetTransitionGUID { get; set; }
        internal WfBackwardTaskReciever BackwardTaskReciever { get; set; }
    }
}
