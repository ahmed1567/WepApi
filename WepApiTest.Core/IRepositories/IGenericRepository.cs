namespace WepApiTest.Core;

public interface IGenericRepository<T>
    where T : class
{
    Task<IList<T>> GetAll();
     
    Task Insert(T entity);
    Task InsertRange(IEnumerable<T> entities);
    void Update(T entity);  

    Task Delete(int id);    
    Task DeletetRange(IEnumerable<T> entities);
    Task<T> GetById(Expression<Func<T, bool>> expression, List<string> includes = null);
    Task<IPagedList<T>> GetPaging(RequestParams requestParams, List<string> includes = null);
}
