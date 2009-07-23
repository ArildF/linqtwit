using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqTwit.Linq
{
    class IdExpressionVisitor : ExpressionVisitor
    {
        public readonly IList<BinaryExpression> Expressions = new List<BinaryExpression>();

        readonly ResolveLocalVisitor _localVisitor = new ResolveLocalVisitor();

        public bool FindIdExpression(LambdaExpression expression)
        {
            this.Visit(expression);

            return this.Expressions.Count > 0;
        }


        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThanOrEqual:
                    expression =
                        (BinaryExpression) _localVisitor.Resolve(expression);

                    this.Expressions.Add(expression);
                    return expression;
                default:
                    return base.VisitBinaryExpression(expression);
            }
        }

    }
}
