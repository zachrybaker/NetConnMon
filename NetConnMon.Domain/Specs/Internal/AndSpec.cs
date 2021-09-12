using System;
using System.Linq.Expressions;

namespace NetConnMon.Domain.Specs.Internal
{
    public class AndSpec<T> : BaseSpecs<T>
    {
        private readonly BaseSpecs<T> _left;
        private readonly BaseSpecs<T> _right;

        public AndSpec(BaseSpecs<T> left, BaseSpecs<T> right)
        {
            _right = right;
            _left  = left;
        }

        public override Expression<Func<T, bool>> FilterConditionExpr => this.GetFilterExpression();

        public Expression<Func<T, bool>> GetFilterExpression()
        {
            Expression<Func<T, bool>> leftExpression  = _left .FilterConditionExpr;
            Expression<Func<T, bool>> rightExpression = _right.FilterConditionExpr;

            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody  = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
            exprBody      = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
            var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);

            return finalExpr;
        }
    }
}
