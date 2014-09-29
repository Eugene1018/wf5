﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 日志记录实体
    /// </summary>
    [Table("WfLog")]
    public class LogEntity
    {
        public int LogID { get; set; }
        public int EventTypeID { get; set; }
        public int Priority { get; set; }
        public string Severity { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerStackTrace { get; set; }
        public string RequestData { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
