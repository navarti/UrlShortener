using Microsoft.EntityFrameworkCore;
using Shortener.Common.Enums;
using System.Linq.Expressions;

using Shortener.Domain.Repositories.Interfaces;
using Shortener.Domain.Entities.Interfaces;

namespace Shortener.Domain.Repositories;

public class EntityRepositoryBase<TKey, TEntity> : IEntityRepository<TKey, TEntity>
    where TEntity : class, IKeyedEntity<TKey>
{
    private readonly ShortenerDbContext dbContext;
    private readonly DbSet<TEntity> dbSet;

    public EntityRepositoryBase(ShortenerDbContext dbContext)
    {
        this.dbContext = dbContext;
        dbSet = this.dbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> Create(TEntity entity)
    {
        await dbSet.AddAsync(entity).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }

    public virtual async Task Delete(TEntity entity)
    {
        dbContext.Entry(entity).State = EntityState.Deleted;

        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual IQueryable<TEntity> Get(
        int skip = 0,
        int take = 0,
        Expression<Func<TEntity, bool>> whereExpression = null,
        Dictionary<Expression<Func<TEntity, object>>, SortDirection> orderBy = null,
        bool asNoTracking = false)
    {
        IQueryable<TEntity> query = dbSet;
        if (whereExpression != null)
        {
            query = query.Where(whereExpression);
        }

        if ((orderBy != null) && orderBy.Any())
        {
            var orderedData = orderBy.Values.First() == SortDirection.Ascending
                ? query.OrderBy(orderBy.Keys.First())
                : query.OrderByDescending(orderBy.Keys.First());

            foreach (var expression in orderBy.Skip(1))
            {
                orderedData = expression.Value == SortDirection.Ascending
                    ? orderedData.ThenBy(expression.Key)
                    : orderedData.ThenByDescending(expression.Key);
            }

            query = orderedData;
        }

        if (skip > 0)
        {
            query = query.Skip(skip);
        }

        if (take > 0)
        {
            query = query.Take(take);
        }

        return asNoTracking ? query.AsNoTracking() : query;
    }

    public virtual async Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> whereExpression)
    {
        var query = dbSet.Where(whereExpression);
        return await query.ToListAsync().ConfigureAwait(false);
    }

    public virtual Task<TEntity> GetById(TKey id) => dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id));

    public virtual async Task<TEntity> Update(TEntity entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;

        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }
}
