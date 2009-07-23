using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
                case ExpressionType.AndAlso:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThanOrEqual:
                    return this.VisitBinaryExpression((BinaryExpression) expr);
                case ExpressionType.Convert:
                    return this.VisitUnaryExpression((UnaryExpression) expr);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression) expr);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression) expr);
                default:
                    return expr;
            }
        }

        protected virtual Expression VisitParameter(ParameterExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression expression)
        {
            return expression;
        }

        private Expression VisitMethodCall(MethodCallExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitUnaryExpression(UnaryExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitBinaryExpression(BinaryExpression expression)
        {
            switch(expression.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                    return Expression.MakeBinary(
                        expression.NodeType,
                        Visit(expression.Left),
                        Visit(expression.Right));
            }
            return expression;
        }

        protected virtual Expression VisitCall(MethodCallExpression expr)
        {
            var args = VisitList(expr.Arguments);

            var method = Visit(expr.Object);

            return Expression.Call(method, expr.Method, args.ToArray());
        }

        private IEnumerable<Expression> VisitList(IEnumerable<Expression> expressions)
        {
            return expressions.Select(e => Visit(e));
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
            return lambda;
        }
    }
}