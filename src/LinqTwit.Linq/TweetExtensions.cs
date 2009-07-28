using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqTwit.Linq
{
    public static class TweetExtensions
    {
        public static IQueryable<ITweet> Page(this IQueryable<ITweet> queryable,
                                              int page)
        {
            return queryable.Provider.CreateQuery<ITweet>(
                Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()), 
                new[] { queryable.Expression, Expression.Constant(page) }));
        }
    }
}
