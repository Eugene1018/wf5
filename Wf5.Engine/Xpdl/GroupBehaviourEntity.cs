﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Xpdl
{
    /// <summary>
    /// 连线上的行为属性实体
    /// </summary>
    public class GroupBehaviourEntity
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public short Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 并行选项
        /// </summary>
        public  ParallelOptionEnum ParallelOption
        {
            get;
            set;
        }
    }
}
