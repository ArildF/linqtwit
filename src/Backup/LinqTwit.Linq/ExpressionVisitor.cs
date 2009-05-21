using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                case ExpressionType.Equal:
                    return this.VisitBinaryExpression((BinaryExpression) expr);
                default:
                    return expr;
            }
        }

        protected virtual Expression VisitBinaryExpression(BinaryExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitCall(MethodCallExpression expr)
        {
            VisitList(expr.Arguments);

            return expr;
        }

        private void VisitList(IEnumerable<Expression> expressions)
        {
            foreach (var expression in expressions)
            {
                Visit(expression);
            }
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
            return lambda;
        }
    }
}