using DocumentEditing.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditing.DAL.Implementations
{
	class DbRepository : IDbRepository
	{
		private readonly AuthDbContext _context;

		public DbRepository(AuthDbContext context)
		{
			_context = context;
		}

		public async Task Add<T>(T newEntity) where T : class
		{
			await _context.Set<T>().AddAsync(newEntity);
		}

		public IQueryable<T> Get<T>() where T : class
		{
			return _context.Set<T>().AsQueryable();
		}

		public async Task Update<T>(T entity) where T : class
		{
			await Task.Run(() => _context.Set<T>().Update(entity));
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
