using System.Linq.Expressions;

namespace LinqTwit.Linq
{
    public interface IQuery
    {
        object Execute(Expression expression, bool isEnumerable);
    }
}