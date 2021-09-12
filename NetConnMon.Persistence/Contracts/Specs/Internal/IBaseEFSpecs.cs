using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace NetConnMon.Persistence.Contracts.Specs.Internal
{
    /// <summary>
    /// See also NetConnMon.Domain.Specs's IBaseSpecs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseEFSpecs<T> 
    {
        //bool ReadOnly { get; }  

        // dropping the Expression allows for "Include.ThenInclude: symmantics which should be more SOLID if ThenInclude is needed.
        // Pros and cons.  The conn is caller (the concrete spec) has to ref efcore dll, and wrangle the expression.
        // Arguably, though, the same could be said about the order/group by, that they should be functions for their flexibility....I digress
        // https://stackoverflow.com/questions/46374252/how-to-write-repository-method-for-theninclude-in-ef-core-2
        Func<IQueryable<T>, IIncludableQueryable<T, object>> IncludeFn { get; }

        //Expression<Func<T, bool>>   FilterConditionExpr   { get; }
        //Expression<Func<T, object>> GroupByExpr           { get; }
        //Expression<Func<T, object>> OrderByExpr           { get; }
        //Expression<Func<T, object>> OrderByDescendingExpr { get; }


        //int? Skip { get; }
        //int? Take { get; }

    }
}

                                          