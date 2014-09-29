using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 活动实例的实体对象
    /// </summary>
    [Table("WfActivityInstance")]
    public class ActivityInstanceEntity
    {
        public int ActivityInstanceID { get; set; }
        public int ProcessInstanceID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityType { get; set; }
        public short ActivityState { get; set; }
        public Nullable<short> GatewayDirectionTypeID { get; set; }
        public byte CanRenewInstance { get; set; }
        public int TokensRequired { get; set; }
        public int TokensHad { get; set; }
        public Nullable<short> ComplexType { get; set; }
        public Nullable<int> MIHostActivityInstanceID { get; set; }
        public Nullable<float> CompleteOrder { get; set; }
        public short BackwardType { get; set; }
        public Nullable<int> BackSrcActivityInstanceID { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public string AssignedToUsers { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public byte IsActivityCompleted { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public byte RecordStatusInvalid { get; set; }
        
    }
}
