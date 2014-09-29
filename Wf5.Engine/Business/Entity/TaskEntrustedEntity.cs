using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wf5.Engine.Business.Entity
{
    /// <summary>
    /// 任务委托实体
    /// </summary>
    public class TaskEntrustedEntity
    {
        public int TaskID { get; set; }
        public int RunnerID { get; set; }
        public string RunnerName { get; set; }
        public int EntrustToUserID { get; set; }
        public string EntrustToUserName { get; set; }
    }
}
