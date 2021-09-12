using System;
using System.Linq;
using System.Linq.Expressions;

namespace NetConnMon.Domain.Specs
{ 
    /// <summary>
    /// Base specifications interface. Extended further in the persistence
    /// project with EF core properties.
    /// But here so we can use it everywhere.
    /// </summary>
    public interface IBaseSpecs<T>
    {
        bool ReadOnly { get; }

        Expression<Func<T, bool>>   FilterConditionExpr   { get; }
        Expression<Func<T, object>> GroupByExpr           { get; }
        Expression<Func<T, object>> OrderByExpr           { get; }
        Expression<Func<T, object>> OrderByDescendingExpr { get; }

        int? Skip { get; }
        int? Take { get; }

        IBaseSpecs<T> SetReadOnly(bool noTracking);

        // these return a new instance by joining it with another instance.
        //IBaseSpecs<T> And(IBaseSpecs<T> specification);
        //IBaseSpecs<T> Or(IBaseSpecs<T> specification);
        // these may not be useful....
        bool IsSatisfiedBy(T entity);
        IBaseSpecs<T> SetOrderBy(Expression<Func<T, object>> orderByExpression);
        IBaseSpecs<T> SetOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression);
        IBaseSpecs<T> SetFilterCondition(Expression<Func<T, bool>> filterExpression);
        IBaseSpecs<T> SetGroupBy(Expression<Func<T, object>> groupByExpression);
        IBaseSpecs<T> SetSkipTake(int? skip, int? take);
        IBaseSpecs<T> SetPagination(int pageSize, int pageNum);
    }
}

                                          