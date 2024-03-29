﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 任务实体对象
    /// </summary>
    [Table("WfTasks")]
    public class TaskEntity
    {
        public int TaskID { get; set; }
        public int ActivityInstanceID { get; set; }
        public int ProcessInstanceID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public System.Guid ActivityGUID { get; set; }
        public string ActivityName { get; set; }
        public short TaskType { get; set; }
        public short TaskState { get; set; }
        public int AssignedToUserID { get; set; }
        public string AssignedToUserName { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public Nullable<int> EndedByUserID { get; set; }
        public string EndedByUserName { get; set; }
        public Nullable<System.DateTime> EndedDateTime { get; set; }
        public byte RecordStatusInvalid { get; set; }
    }
}
