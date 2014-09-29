using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wf5.Engine.Data
{
    public class RepositoryFactory
    {
        private readonly static object _lock = new object();
        private static Repository _instance;
        public static Repository CreateRepository()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Repository();
                    }
                }
            }
            return _instance;
        }
    }
}
