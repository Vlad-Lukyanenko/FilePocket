﻿using FilePocket.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected FilePocketDbContext DbContext;

    public RepositoryBase(FilePocketDbContext repositoryContext)
    {
        DbContext = repositoryContext;
    }

    public IQueryable<T> FindAll(bool trackChanges)
    {
        return trackChanges ? DbContext.Set<T>()
                            : DbContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
    {
        return trackChanges ? DbContext.Set<T>().Where(expression)
                            : DbContext.Set<T>().Where(expression).AsNoTracking();
    }

    public Task<T?> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return DbContext.Set<T>().FirstOrDefaultAsync(expression);
    }

    public void Create(T entity)
    {
        DbContext.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        DbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        DbContext.Set<T>().Remove(entity);
    }

    public void DeleteAll(IEnumerable<T> entities)
    {
        DbContext.Set<T>().RemoveRange(entities);
    }
}
