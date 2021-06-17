using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.DAL.Contracts
{
	public interface IDbRepository
	{
		IQueryable<T> Get<T>() where T : class;		

		Task Add<T>(T newEntity) where T : class;

		Task Update<T>(T entity) where T : class;

		Task SaveChangesAsync();
	}
}
