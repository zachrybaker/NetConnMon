using System;
using System.Linq.Expressions;

namespace NetConnMon.Domain.Specs.Internal
{
    public class ParameterReplacer : ExpressionVisitor
    {

        private readonly ParameterExpression _parameter;

        protected override Expression VisitParameter(ParameterExpression node)
            => base.VisitParameter(_parameter);

        internal ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }
    }
}
