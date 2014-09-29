using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Mapper;

namespace Wf5.Engine.Business.Entity
{
    [Table("WfProcessInstance")]
    public class ProcessInstanceEntity
    {
        public int ProcessInstanceID { get; set; }
        public Guid ProcessGUID { get; set; }
        public string ProcessName { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public string AppInstanceCode { get; set; }
        public short ProcessState { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Nullable<int> ParentProcessInstanceID { get; set; }
        public Nullable<Guid> ParentProcessGUID { get; set; }
        public int InvokedActivityInstanceID { get; set; }
        public Nullable<Guid> InvokedActivityGUID { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public byte IsProcessCompleted { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public byte RecordStatusInvalid { get; set; }
    }
}
