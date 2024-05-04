using Shortener.Common.Enums;
using Shortener.Domain.Entities;
using System.Linq.Expressions;

namespace Shortener.WebApi.Util.Filters;

public class UrlPairFilter
{
    public int Skip = 0;
    public int Take = 0;
    public Expression<Func<UrlPair, bool>> WhereExpression = null;
    public Dictionary<Expression<Func<UrlPair, object>>, SortDirection> OrderBy = null;
    public bool AsNoTracking = false;
}
