// Repositories/Repository.cs
using Employees_Salary_Hub.Data;
using Microsoft.EntityFrameworkCore;
using Employees_Salary_Hub.Repositories.Interfaces;


namespace Employees_Salary_Hub.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) { _dbSet.Remove(entity); await _context.SaveChangesAsync(); }
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _dbSet.FindAsync(id) != null;
    }
}
