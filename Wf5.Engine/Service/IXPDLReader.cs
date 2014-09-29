using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wf5.Engine.Business.Entity;

namespace Wf5.Engine.Service
{
    public interface IXPDLReader
    {
        bool IsReadable();
        XmlDocument Read(ProcessEntity entity);
    }
}
