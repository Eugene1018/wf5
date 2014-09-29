using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wf5.WebDemo.Entity
{
    public class UserInfo
    {
        private int _uID;
        private string _uName;
        private int _uRoleId;


        public int uID
        {
            get { return _uID; }
            set { _uID = value; }
        }


        public string uName
        {
            get { return _uName; }
            set { _uName = value; }
        }

        public int uRoleId
        {
            get { return _uRoleId; }
            set { _uRoleId = value; }
        }
    }
}