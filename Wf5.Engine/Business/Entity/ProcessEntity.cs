using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 流程实体类
    /// </summary>
    [Table("WfProcess")]
    public class ProcessEntity
    {
        public System.Guid ProcessGUID { get; set; }
        public string ProcessName { get; set; }
        public string PageUrl { get; set; }
        public string XmlFileName { get; set; }
        public string XmlFilePath { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateTime { get; set; }
    }
}
