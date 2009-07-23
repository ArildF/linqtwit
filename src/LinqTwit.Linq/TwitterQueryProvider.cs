using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class TwitterQueryProvider : IQueryProvider
    {
        private readonly ILinqApi linqApi;
        private readonly Func<IQuery> _createQuery;

        public TwitterQueryProvider(ILinqApi linqApi, Func<IQuery> createQuery)
        {
            this.linqApi = linqApi;
            _createQuery = createQuery;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TwitterQueryable<TElement>(linqApi, this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            IQuery query = _createQuery();

            return (TResult)query.Execute(expression, isEnumerable);
        }
    }
}
