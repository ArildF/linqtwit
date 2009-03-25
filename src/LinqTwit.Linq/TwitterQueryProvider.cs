using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqTwit.Linq
{
    public class TwitterQueryProvider : IQueryProvider
    {
        private readonly ILinqApi linqApi;

        public TwitterQueryProvider(ILinqApi linqApi)
        {
            this.linqApi = linqApi;
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
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            TwitterQuery query = new TwitterQuery(expression, IsEnumerable, this.linqApi);
            return (TResult)query.Execute();
        }
    }
}
