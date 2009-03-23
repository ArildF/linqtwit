using System;
using System.Linq.Expressions;

namespace LinqTwit.Linq
{
    internal class ExpressionVisitor
    {
        protected virtual Expression Visit(Expression expr)
        {
            if (expr == null)
            {
                return null;
            }

            switch (expr.NodeType)
            {
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression) expr);
                case ExpressionType.Call:
                    return this.VisitCall((MethodCallExpression) expr);
                default:
                    return expr;
            }
        }

        protected virtual Expression VisitCall(MethodCallExpression expr)
        {
            return expr;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            return lambda;
        }
    }
}