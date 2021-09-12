using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using NetConnMon.Domain;
using NetConnMon.Domain.Specs;

namespace NetConnMon.Persistence.Contracts.Specs.Internal
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> query,
            IBaseSpecs<TEntity> linqSpecs,
            IBaseEFSpecs<TEntity> efSpecs)
        {
            if (linqSpecs != null)
            {
                if (linqSpecs.ReadOnly)
                    query = query.AsNoTracking();


                // Includes
                if (efSpecs.IncludeFn != null)
                    query = efSpecs.IncludeFn(query);
                // Apply ordering
                if (linqSpecs.OrderByExpr != null)
                    query = query.OrderBy(linqSpecs.OrderByExpr);
                else if (linqSpecs.OrderByDescendingExpr != null)
                    query = query.OrderByDescending(linqSpecs.OrderByDescendingExpr);

                // Apply GroupBy
                if (linqSpecs.GroupByExpr != null)
                    query = query.GroupBy(linqSpecs.GroupByExpr).SelectMany(x => x);

                // Apply Pagination
                if (linqSpecs.Skip.HasValue)
                    query = query.Skip(linqSpecs.Skip.Value);
                if (linqSpecs.Take.HasValue)
                    query = query.Take(linqSpecs.Take.Value);
            }

            // Modify the IQueryable
            // Apply filter conditions
            if (linqSpecs != null && linqSpecs.FilterConditionExpr != null)
                query = query.Where(linqSpecs.FilterConditionExpr);

            return query;
        }
    }
}
