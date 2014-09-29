using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Business;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 任务视图类
    /// </summary>
    [Table("vwWfActivityInstanceTasks")]
    public class TaskViewEntity
    {
        public int TaskID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public int ProcessInstanceID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public int ActivityInstanceID { get; set; }
        public string ActivityName { get; set; }
        public short ActivityType { get; set; }
        public short TaskType { get; set; }
        public int AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public short TaskState { get; set; }
        public short ActivityState { get; set; }
        public byte IsActivityCompleted { get; set; }
        public byte RecordStatusInvalid { get; set; }
        public short ProcessState { get; set; }
    }
}
