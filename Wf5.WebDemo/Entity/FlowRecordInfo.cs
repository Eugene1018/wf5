using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wf5.WebDemo.Entity
{
    public class FlowRecordInfo
    {
        /// <summary>
        /// ID
        /// </summary>		
        private int _frid;
        public int frID
        {
            get { return _frid; }
            set { _frid = value; }
        }

        /// <summary>
        /// XML流程定义GUID
        /// </summary>		
        private Guid _frprocessguid;
        public Guid frProcessGuid
        {
            get { return _frprocessguid; }
            set { _frprocessguid = value; }
        }

        /// <summary>
        /// WfProcessInstance.ProcessInstanceID
        /// </summary>		
        private int _frprocessinstanceid;
        public int frProcessInstanceID
        {
            get { return _frprocessinstanceid; }
            set { _frprocessinstanceid = value; }
        }

        /// <summary>
        /// WfProcessInstance.AppInstanceID(业务表ID)
        /// </summary>		
        private int _frappinstanceid;
        public int frAppInstanceID
        {
            get { return _frappinstanceid; }
            set { _frappinstanceid = value; }
        }

        /// <summary>
        /// WfActivityInstance.ActivityInstanceID
        /// </summary>		
        private int _fractivityinstanceid;
        public int frActivityInstanceID
        {
            get { return _fractivityinstanceid; }
            set { _fractivityinstanceid = value; }
        }

        /// <summary>
        /// XML活动定义GUID
        /// </summary>		
        private Guid _fractivityguid;
        public Guid frActivityGuid
        {
            get { return _fractivityguid; }
            set { _fractivityguid = value; }
        }

        /// <summary>
        /// 活动类型
        /// </summary>		
        private int _fractivitytype;
        public int frActivityType
        {
            get { return _fractivitytype; }
            set { _fractivitytype = value; }
        }

        /// <summary>
        /// 活动状态
        /// </summary>		
        private int _fractivitystate;
        public int frActivityState
        {
            get { return _fractivitystate; }
            set { _fractivitystate = value; }
        }

        /// <summary>
        /// 办理意见
        /// </summary>		
        private string _frdealadvice;
        public string frDealAdvice
        {
            get { return _frdealadvice; }
            set { _frdealadvice = value; }
        }

        /// <summary>
        /// 附件
        /// </summary>		
        private string _frfile;
        public string frFile
        {
            get { return _frfile; }
            set { _frfile = value; }
        }

        /// <summary>
        /// 签字使用
        /// </summary>		
        private string _frsigninfo;
        public string frSignInfo
        {
            get { return _frsigninfo; }
            set { _frsigninfo = value; }
        }

        /// <summary>
        /// 操作人
        /// </summary>		
        private int _froperator;
        public int frOperator
        {
            get { return _froperator; }
            set { _froperator = value; }
        }

        /// <summary>
        /// 操作时间
        /// </summary>		
        private DateTime _froperatortime;
        public DateTime frOperatorTime
        {
            get { return _froperatortime; }
            set { _froperatortime = value; }
        }
    }
}