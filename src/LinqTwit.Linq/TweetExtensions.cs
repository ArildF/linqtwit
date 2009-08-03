using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using LinqTwit.Twitter;

namespace LinqTwit.Linq
{
    public static class TweetExtensions
    {
        public static IQueryable<Status> Page(this IQueryable<Status> queryable,
                                              int page)
        {
            return queryable.Provider.CreateQuery<Status>(
                Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod()), 
                new[] { queryable.Expression, Expression.Constant(page) }));
        }
    }
}
