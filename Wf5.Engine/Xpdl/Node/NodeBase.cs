using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Common;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Xpdl.Node
{
    /// <summary>
    /// 节点的基类
    /// </summary>
    public abstract class NodeBase
    {
        #region 属性和构造函数

        
        internal ProcessModel _processModel;
        internal ProcessModel ProcessModel
        {
            get
            {
                if (_processModel == null)
                {
                    _processModel = new ProcessModel(this.Activity.ProcessGUID);
                }
                return _processModel;
            }
        }

        /// <summary>
        /// 节点定义属性
        /// </summary>
        public ActivityEntity Activity
        {
            get;
            set;
        }

        /// <summary>
        /// 节点实例
        /// </summary>
        public ActivityInstanceEntity ActivityInstance
        {
            get;
            set;
        }
        

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currentActivity"></param>
        public NodeBase(ActivityEntity currentActivity)
        {
            Activity = currentActivity;
        }
        #endregion
    }
}
