using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetConnMon.Domain.Specs.Internal;

namespace NetConnMon.Domain.Specs
{
    public class BaseSpecs<T> : IBaseSpecs<T>
    {
        public BaseSpecs() { }

        public BaseSpecs(Expression<Func<T, bool>> filterCondition)
         => this.FilterConditionExpr = filterCondition;

        public IBaseSpecs<T> SetReadOnly(bool noTracking)
        {
            ReadOnly = noTracking;
            return this;
        }
        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = this.FilterConditionExpr.Compile();
            return predicate(entity);
        }

        public BaseSpecs<T> And(BaseSpecs<T> specification)
            => new AndSpec<T>(this, specification);
        
        public BaseSpecs<T> Or(BaseSpecs<T> specification)
            => new OrSpec<T>(this, specification);

        public bool ReadOnly { get; private set; }
        public int? Skip { get; private set; }
        public int? Take { get; private set; }
        public virtual Expression<Func<T, bool>> FilterConditionExpr   { get; private set; }
        public Expression<Func<T, object>>       OrderByExpr           { get; private set; }
        public Expression<Func<T, object>>       OrderByDescendingExpr { get; private set; }
        public Expression<Func<T, object>>       GroupByExpr           { get; private set; }


        public IBaseSpecs<T> SetOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderByExpr = orderByExpression;
            return this;
        }
        public IBaseSpecs<T> SetOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescendingExpr = orderByDescendingExpression;
            return this;
        }
        public IBaseSpecs<T> SetFilterCondition(Expression<Func<T, bool>> filterExpression)
        {
            FilterConditionExpr = filterExpression;
            return this;
        }
        public IBaseSpecs<T> SetGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupByExpr = groupByExpression;
            return this;
        }

        public IBaseSpecs<T> SetSkipTake(int? skip, int? take)
        {
            Skip = skip;
            Take = take;
            return this;
        }
        public IBaseSpecs<T> SetPagination(int pageSize, int pageNum)
        {
            Skip = pageNum * pageSize;
            Take = pageSize;
            return this;
        }
    }
}
