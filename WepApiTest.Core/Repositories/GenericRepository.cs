using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using X.PagedList;

namespace WepApiTest.Core;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    private readonly ApplicationDbContext _context;

    private readonly DbSet<T> dbSet;


    public GenericRepository(ApplicationDbContext Context)
    {
        
        _context = Context;
        dbSet=_context.Set<T>(); 
    }
    public  async Task Delete(int id)
    {
        var entity= await dbSet.FindAsync(id);
        dbSet.Remove(entity);
        
    }

    public async Task DeletetRange(IEnumerable<T> entities)
    {
        await Task.Run( () =>  dbSet.RemoveRange(entities));
        
    }

    public async Task<IList<T>> GetAll() => await dbSet.ToListAsync();

    public async Task<IPagedList<T>> GetPaging(RequestParams requestParams ,List<string> includes = null )
    {
        IQueryable<T> query = dbSet;

        if (includes != null)
        {
            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }
        }
        return await query.AsNoTracking().ToPagedListAsync(requestParams.PageNumber,requestParams.PageSize);
    }

    public async Task<T> GetById(Expression<Func<T, bool>> expression ,List<string> includes =null) {
        IQueryable<T> query = dbSet;

        if(includes != null)
        {
            foreach(var includeProperty in includes) {
                    query = query.Include(includeProperty);
            }
        }
        return await query.FirstOrDefaultAsync(expression);
    }


    public async Task Insert(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public async Task InsertRange(IEnumerable<T> entities)
    {
        await dbSet.AddRangeAsync(entities);    
        
    }

    public void Update(T entity)
    {
        dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }


}
