using System;
using System.Linq.Expressions;

namespace NetConnMon.Domain.Specs
{
    public static class SpecExtensions
    {
        public static BaseSpecs<T> AsReadOnly<T>(this BaseSpecs<T> specs, bool noTracking)
        {
            specs.SetReadOnly(noTracking);
            return specs;
        }
        public static BaseSpecs<T> Where<T>(this BaseSpecs<T> specs, Expression<Func<T, bool>> filter)
        {

            return specs;
        }
        public static BaseSpecs<T> OrderBy<T>(this BaseSpecs<T> specs, Expression<Func<T, object>> orderByExpression)
        {
            specs.SetOrderBy(orderByExpression);
            return specs;
        }
        public static BaseSpecs<T> OrderByDescending<T>(this BaseSpecs<T> specs, Expression<Func<T, object>> orderByDescendingExpression)
        {
            specs.SetOrderByDescending(orderByDescendingExpression);
            return specs;
        }
        public static BaseSpecs<T> Filter<T>(this BaseSpecs<T> specs, Expression<Func<T, bool>> filterExpression)
        {
            specs.SetFilterCondition(filterExpression);
            return specs;
        }

        public static BaseSpecs<T> GroupBy<T>(this BaseSpecs<T> specs, Expression<Func<T, object>> groupByExpression)
        {
            specs.SetGroupBy(groupByExpression);
            return specs;
        }
        public static BaseSpecs<T> SkipTake<T>(this BaseSpecs<T> specs, int? skip, int? take)
        {
            specs.SetSkipTake(skip, take);
            return specs;
        }
        public static BaseSpecs<T> Paginate<T>(this BaseSpecs<T> specs, int pageSize, int pageNum)
        {
            specs.SetSkipTake(pageNum * pageSize, pageSize);
            return specs;
        }
    }
}
