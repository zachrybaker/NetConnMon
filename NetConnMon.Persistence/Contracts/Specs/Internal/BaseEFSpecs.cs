using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using NetConnMon.Domain.Specs;

namespace NetConnMon.Persistence.Contracts.Specs.Internal
{
    // Generic Specifications, for filter expressions.
    // For additional expressions, derive from BaseSpecs.
    // Pattern is derived roughly from a mix of the folllowing two examples:
    // https://github.com/jbogard/MediatR/issues/521#issuecomment-897131719
    // https://github.com/manoj-choudhari-git/dotnet-on-thecodeblogger/tree/main/EFCoreBlogDemo (looks cribbed from somewhere else)
    public class BaseEFSpecs<T> : //BaseSpecs<T>, , IBaseSpecs<T>
    IBaseEFSpecs<T>
    {
        public BaseEFSpecs() :base() { }

        //public BaseEFSpecs(Expression<Func<T, bool>> filterCondition) : base(filterCondition) { }
        public BaseEFSpecs(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeFn)
            => _IncludeFn = includeFn;

        public  Func<IQueryable<T>, IIncludableQueryable<T, object>> IncludeFn => _IncludeFn;
        private Func<IQueryable<T>, IIncludableQueryable<T, object>> _IncludeFn = null;

        protected void SetIncludeFn(Func<IQueryable<T>, IIncludableQueryable<T, object>> vn)
        {
            _IncludeFn = vn;
        }
    }
}
