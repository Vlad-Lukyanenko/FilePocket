using System.Linq.Expressions;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll(bool trackChanges);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);

    Task<T?> FindByCondition(Expression<Func<T, bool>> expression);

    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);

    void DeleteAll(IEnumerable<T> entities);
}
