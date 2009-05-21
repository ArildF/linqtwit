using System;
using System.Linq.Expressions;
using LinqTwit.Utilities;

namespace LinqTwit.Linq
{
    class IdExpressionVisitor : ExpressionVisitor
    {
        public bool FindIdExpression(LambdaExpression expression)
        {
            this.Visit(expression);

            return this.TweetId != null;
        }

        public string TweetId { get; private set; }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Equal && expression.Left.NodeType == ExpressionType.MemberAccess)
            {
                var memberAccess = (MemberExpression)expression.Left;
                if (memberAccess.Member.IsProperty<ITweet>(tweet => tweet.Id))
                {
                    if (expression.Right.NodeType == ExpressionType.Constant)
                    {
                        var rhs = (ConstantExpression)expression.Right;
                        if (rhs.Type == typeof(string))
                        {
                            this.TweetId = (string)rhs.Value;
                        }
                    }

                }
            }

            return expression;
        }
    }
}
