
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.Web.Models;

namespace UdemyRealWorldUnitTest.Web.Repository
{
    public class  Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly UdemyRealWorldUnitTestContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(UdemyRealWorldUnitTestContext context)
        {
            _context=context;
            _dbSet=_context.Set<TEntity>();
        }

        public async Task Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        //async yok çünkü entity.state= ile işlem yapar.
        public void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        //async yok çünkü entity.state= ile işlem yapar.
        public void Delete(TEntity entity)
        {
            //_context.Entry(entity).State = EntityState.Deleted;
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

    }
}
