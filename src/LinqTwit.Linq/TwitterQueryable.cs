﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public class TwitterQueryable<T> : IQueryable<T>
    {
        public TwitterQueryable(IQueryProvider provider)
        {
            Provider = provider;
            Expression = Expression.Constant(this);

        }

        public TwitterQueryable(TwitterQueryProvider provider, Expression expression)
        {
            this.Expression = expression;
            this.Provider = provider;
        }

        public Expression Expression
        {
            get; private set;
        }

        public IQueryProvider Provider { get; private set; }

        public Type ElementType
        {
            get { return typeof (T); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return
                Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
