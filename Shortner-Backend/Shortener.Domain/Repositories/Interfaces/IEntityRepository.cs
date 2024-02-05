using System.Linq.Expressions;

using Shortener.Common.Enums;

namespace Shortener.Domain.Repositories.Interfaces;

public interface IEntityRepository<TKey, TEntity>
{
    Task<TEntity> Create(TEntity entity);

    IQueryable<TEntity> Get(
        int skip = 0,
        int take = 0,
        Expression<Func<TEntity, bool>> whereExpression = null,
        Dictionary<Expression<Func<TEntity, object>>, SortDirection> orderBy = null,
        bool asNoTracking = false);

    Task<TEntity> GetById(TKey id);

    Task<TEntity> Update(TEntity entity);

    Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> whereExpression);

    Task Delete(TEntity entity);
}
