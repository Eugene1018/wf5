using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wf5.WebDemo.Entity
{
    public class FlowInstanceInfo
    {
        /// <summary>
        /// fID
        /// </summary>		
        private int _fiid;
        public int fiID
        {
            get { return _fiid; }
            set { _fiid = value; }
        }

        /// <summary>
        /// WorkFlow_FlowDefinition表的Guid
        /// </summary>		
        private Guid _fiflowdefinitionguid;
        public Guid fiFlowDefinitionGuid
        {
            get { return _fiflowdefinitionguid; }
            set { _fiflowdefinitionguid = value; }
        }

        /// <summary>
        /// 1-表单流程 2-业务模块流程
        /// </summary>		 
        private string _fitype;
        public string fiType
        {
            get { return _fitype; }
            set { _fitype = value; }
        }

        /// <summary>
        /// 具体的业务ID 
        /// [流程为表单时值为表单ID、流程为业务时值为具体的业务表ID]
        /// </summary>		
        private int _fiobjectid;
        public int fiObjectId
        {
            get { return _fiobjectid; }
            set { _fiobjectid = value; }
        }

        /// <summary>
        /// 业务编号
        /// </summary>		
        private string _fiserialno;
        public string fiSerialNo
        {
            get { return _fiserialno; }
            set { _fiserialno = value; }
        }

        /// <summary>
        /// 业务流程编号
        /// </summary>		
        private string _fino;
        public string fiNo
        {
            get { return _fino; }
            set { _fino = value; }
        }

        /// <summary>
        /// 业务流程编号标识符
        /// </summary>		
        private string _finopattern;
        public string fiNoPattern
        {
            get { return _finopattern; }
            set { _finopattern = value; }
        }


        /// <summary>
        /// 流程状态
        /// </summary>		
        private string _fistate;
        public string fiState
        {
            get { return _fistate; }
            set { _fistate = value; }
        }

        /// <summary>
        /// 流程重要性
        /// </summary>		
        private string _filevel;
        public string fiLevel
        {
            get { return _filevel; }
            set { _filevel = value; }
        }

        /// <summary>
        /// 流程标题
        /// </summary>		
        private string _fititle;
        public string fiTitle
        {
            get { return _fititle; }
            set { _fititle = value; }
        }

        /// <summary>
        /// 申请理由
        /// </summary>		
        private string _fiapplyreason;
        public string fiApplyReason
        {
            get { return _fiapplyreason; }
            set { _fiapplyreason = value; }
        }

        /// <summary>
        /// 流程发起人
        /// </summary>		
        private string _fiapplyer;
        public string fiApplyer
        {
            get { return _fiapplyer; }
            set { _fiapplyer = value; }
        }

        /// <summary>
        /// 流程发起时间
        /// </summary>		
        private DateTime _fiapplytime;
        public DateTime fiApplyTime
        {
            get { return _fiapplytime; }
            set { _fiapplytime = value; }
        }

        /// <summary>
        /// 最后操作人
        /// </summary>		
        private string _filastoperator;
        public string fiLastOperator
        {
            get { return _filastoperator; }
            set { _filastoperator = value; }
        }

        /// <summary>
        /// 最后操作时间
        /// </summary>		
        private DateTime _filastopttime;
        public DateTime fiLastOptTime
        {
            get { return _filastopttime; }
            set { _filastopttime = value; }
        }

    }
}