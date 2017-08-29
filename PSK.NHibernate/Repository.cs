using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using PSK.Model;
using PSK.Model.Abstract;

namespace PSK.NHibernate
{
	public class Repository : IRepository
	{
		private readonly Lazy<ISession> _session;


		public Repository(Lazy<ISession> session)
		{
			_session = session;
		}

		public void Close()
		{
			if (_session.IsValueCreated)
				_session.Value.Close();
		}

		//public T GetById<T>(int id) where T : EntityId<int>
		//{
		//	return _session.Value.Query<T>().First(x => x.Id == id);
		//}

		public IQueryable<T> GetAll<T>()
		{
			return _session.Value.Query<T>();
		}

		public void SaveOrUpdate<T>(T entity)
		{
			Transact(() => _session.Value.SaveOrUpdate(entity));
		}

		public void Delete<T>(T entity)
		{
			Transact(() => _session.Value.Delete(entity));
		}

		protected void Transact(Action func)
		{
			if (!_session.Value.Transaction.IsActive)
			{
				using (var tx = _session.Value.BeginTransaction())
				{
					func.Invoke();
					tx.Commit();
				}
			}
			else
				func.Invoke();
		}
	}
}