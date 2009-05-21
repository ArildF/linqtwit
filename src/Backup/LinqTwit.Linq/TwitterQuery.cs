using System.Linq;
using System.Linq.Expressions;
using LinqTwit.Linq.Impl;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class TwitterQuery
    {
        private readonly Expression expression;
        private readonly bool isEnumerable;
        private readonly ILinqApi linqApi;

        public TwitterQuery(Expression expression, bool isEnumerable, ILinqApi linqApi)
        {
            this.expression = expression;
            this.linqApi = linqApi;
            this.isEnumerable = isEnumerable;
        }

        public object Execute()
        {
            var whereVisitor = new WhereVisitor();
            var methodCallExpression = whereVisitor.FindWhere(this.expression);

            LambdaExpression lambdaExpression =
                (LambdaExpression)
                ((UnaryExpression) methodCallExpression.Arguments[1]).Operand;

            var idFinder = new IdExpressionVisitor();
            if(idFinder.FindIdExpression(lambdaExpression))
            {
                string id = idFinder.TweetId;
                Status status = linqApi.GetStatus(id);

                if (isEnumerable)
                {
                    return new[] {status}.Select(s => new Tweet(s)).Cast<ITweet>();
                }
                return status;
            }

            return null;



        }
    }
}