using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Wf5.Engine.Core.Rule
{
    public class IntWrapper
    {
        public int Value
        {
            get;
            set;
        }

        public IntWrapper(int value)
        {
            Value = value;
        }
    }
}
