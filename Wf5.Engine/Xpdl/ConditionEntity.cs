using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Xpdl
{
    /// <summary>
    /// Transition上的条件类
    /// </summary>
    public class ConditionEntity
    {
        /// <summary>
        /// 条件类型
        /// </summary>
        public ConditionTypeEnum ConditionType
        {
            get;
            set;
        }

        /// <summary>
        /// 条件表达式
        /// </summary>
        public string ConditionText
        {
            get;
            set;
        }
    }
}
