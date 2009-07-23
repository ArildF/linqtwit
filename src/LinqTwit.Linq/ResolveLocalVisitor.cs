using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqTwit.Linq
{
    class ResolveLocalVisitor : ExpressionVisitor
    {
        public Expression Resolve(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    var operand = (ConstantExpression)Visit(expression.Operand);
                    return
                        Expression.Constant(
                            ((IConvertible)operand.Value).ToType(
                                expression.Type, null));

                default:
                    return base.VisitUnaryExpression(expression);
            }

        }

        protected override Expression VisitMemberAccess(MemberExpression expression)
        {
            var expr = Visit(expression.Expression);

            if (expr.NodeType != ExpressionType.Constant)
            {
                return expression;
            }

            ConstantExpression obj = (ConstantExpression)expr;

            if (expression.Member.MemberType == MemberTypes.Field)
            {
                FieldInfo pi = (FieldInfo)expression.Member;
                return Expression.Constant(pi.GetValue(obj.Value));
            }
            if (expression.Member.MemberType == MemberTypes.Property)
            {
                PropertyInfo pi = (PropertyInfo)expression.Member;
                return Expression.Constant(pi.GetValue(obj.Value, null));
            }


            return base.VisitMemberAccess(expression);
        }

        protected override Expression VisitCall(MethodCallExpression expr)
        {
            expr = (MethodCallExpression)base.VisitCall(expr);

            if (expr.Arguments.Any(a => a.NodeType != ExpressionType.Constant))
            {
                throw new NotSupportedException("Unresolved arguments remain: " +
                    expr.Arguments
                    .Select(a => a.ToString())
                    .Aggregate((a1, a2) => a1 + ", " + a2));
            }

            if (expr.Object.NodeType != ExpressionType.Constant)
            {
                throw new NotFiniteNumberException("Unresolved object: " + expr.Object);
            }

            var obj = (ConstantExpression)expr.Object;
            var args = expr.Arguments
                .Cast<ConstantExpression>()
                .Select(e => e.Value);

            return Expression.Constant(expr.Method.Invoke(obj.Value, args.ToArray()));


        }

    }
}
