using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Xpdl.Node;

namespace Wf5.Engine.Xpdl
{
    /// <summary>
    /// 活动节点属性定义
    /// </summary>
    public class ActivityEntity
    {
        public Guid ActivityGUID { get; set; }
        public Guid ProcessGUID { get; set; }
        public ActivityTypeEnum ActivityType{ get; set; }
        public ActivityTypeDetail ActivityTypeDetail { get; set; }
        public NodeBase Node { get; set; }

        internal bool IsAutomanticWorkItem
        {
            get
            {
                if ((TaskImplementDetail != null)
                    && ((TaskImplementDetail.ImplementationType | ImplementationTypeEnum.Automantic)
                    == ImplementationTypeEnum.Automantic))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal bool IsSimpleWorkItem
        {
            get
            {
                if ((ActivityType | ActivityTypeEnum.SimpleWorkItem) == ActivityTypeEnum.SimpleWorkItem)
                    return true;
                else
                    return false;
            }
        }

        public GatewaySplitJoinTypeEnum GatewaySplitJoinType { get; set; }
        public GatewayDirectionEnum GatewayDirectionType { get; set; }
        public string ActivityName { get; set; }
        public string Description { get; set; }
        public TaskImplementDetail TaskImplementDetail { get; set; }

        public IList<int> _roles;
        public IList<int> Roles
        {
            get
            {
                if (_roles == null)
                {
                    var processModel = new ProcessModel(ProcessGUID);
                    _roles = processModel.GetActivityRoles(ActivityGUID);
                }
                return _roles;
            }
        }
    }
}
