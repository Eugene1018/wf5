using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wf5.Engine.Common;
using Wf5.Engine.Utility;
using Wf5.Engine.Data;
using Wf5.Engine.Business.Entity;
using Wf5.Engine.Business.Manager;
using Wf5.Engine.Xpdl;

namespace Wf5.Engine.Business.Manager
{
    /// <summary>
    /// 流程定义管理类
    /// </summary>
    public class ProcessManager
    {
        #region 属性、构造函数
        private Repository _processRepository;
        private Repository ProcessRepository
        {
            get
            {
                if (_processRepository == null)
                {
                    _processRepository = RepositoryFactory.CreateRepository();
                }
                return _processRepository;
            }
        }

        public ProcessManager()
        {
        }
        #endregion

        #region 获取流程数据
        public ProcessEntity GetById(Guid processGUID)
        {
            return ProcessRepository.GetById<ProcessEntity>(processGUID);
        }
        #endregion

        #region 新增、更新和删除流程数据
        public void Insert(ProcessEntity entity)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                ProcessRepository.Insert<ProcessEntity>(session.Connection, entity, session.Transaction);
                session.CommitTransaction();
            }
            catch (System.Exception)
            {
                session.RollbackTransaction();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        public void Update(ProcessEntity entity)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                session.BeginTransaction();
                ProcessRepository.Update<ProcessEntity>(session.Connection, entity, session.Transaction);
                session.CommitTransaction();
            }
            catch (System.Exception)
            {
                session.RollbackTransaction();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }

        public void Delete(Guid processGUID)
        {
            IDbSession session = SessionFactory.CreateSession();
            try
            {
                var entity = GetById(processGUID);
                ProcessRepository.Delete<ProcessEntity>(session.Connection, entity, session.Transaction);
                session.CommitTransaction();
            }
            catch (System.Exception)
            {
                session.RollbackTransaction();
                throw;
            }
            finally
            {
                session.Dispose();
            }
        }
        #endregion 
    }
}
