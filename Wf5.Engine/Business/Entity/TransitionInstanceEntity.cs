using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 节点转移类
    /// </summary>
    [Table("WfTransitionInstance")]
    public class TransitionInstanceEntity
    {
        public int TransitionInstanceID { get; set; }
        public System.Guid TransitionGUID { get; set; }
        public string AppName { get; set; }
        public int AppInstanceID { get; set; }
        public int ProcessInstanceID { get; set; }
        public System.Guid ProcessGUID { get; set; }
        public byte TransitionType { get; set; }
        public byte FlyingType { get; set; }
        public int FromActivityInstanceID { get; set; }
        public System.Guid FromActivityGUID { get; set; }
        public short FromActivityType { get; set; }
        public string FromActivityName { get; set; }
        public int ToActivityInstanceID { get; set; }
        public System.Guid ToActivityGUID { get; set; }
        public short ToActivityType { get; set; }
        public string ToActivityName { get; set; }
        public byte ConditionParseResult { get; set; }
        public int CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public byte RecordStatusInvalid { get; set; }
    }
}
