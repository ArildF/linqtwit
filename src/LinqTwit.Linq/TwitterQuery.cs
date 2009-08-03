using System.Linq;
using System.Linq.Expressions;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class TwitterQuery : IQuery
    {
        private readonly ILinqApi _linqApi;

        public TwitterQuery( ILinqApi linqApi)
        {
            this._linqApi = linqApi;
        }

        public object Execute(Expression expression, bool isEnumerable)
        {
            var whereVisitor = new WhereVisitor();
            var methodCallExpression = whereVisitor.FindWhere(expression);

            LambdaExpression lambdaExpression =
                (LambdaExpression)
                ((UnaryExpression) methodCallExpression.Arguments[1]).Operand;

            var idFinder = new IdExpressionVisitor();
            if(idFinder.FindIdExpression(lambdaExpression))
            {
                string id = IdFromExpression(idFinder.Expressions.First());

                Status status = _linqApi.GetStatus(id);

                if (isEnumerable)
                {
                    return new[] {status};
                }
                return status;
            }

            return null;



        }

        private string IdFromExpression(BinaryExpression expression)
        {
            if (expression.NodeType != ExpressionType.Equal)
            {
                throw new InvalidQueryException("");
            }

            return expression.Right.ToString();
        }
    }
}