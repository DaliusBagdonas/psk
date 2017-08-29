using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSK.Model;
using PSK.Model.Abstract;

namespace PSK.NHibernate
{
	public interface IRepository
	{
		IQueryable<T> GetAll<T>();
		void SaveOrUpdate<T>(T entity);
		void Delete<T>(T entity);
		//T GetById<T>(int id) where T : 
	}
}
