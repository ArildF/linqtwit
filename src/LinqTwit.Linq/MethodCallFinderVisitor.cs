using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqTwit.Utilities;

namespace LinqTwit.Linq
{
    class MethodCallFinderVisitor : ExpressionVisitor
    {
        private readonly MethodInfo _method;
        private object[] _args;

        public MethodCallFinderVisitor(MethodInfo method)
        {
            _method = method;
        }

        public IEnumerable<object> Args
        {
            get { return this._args; }
        }


        protected override Expression VisitCall(MethodCallExpression call)
        {
            if (call.Method != _method)
            {
                return call;
            }

            ResolveLocalVisitor rlv = new ResolveLocalVisitor();

            // first arg is the IQueryable, so skip it
            _args = call.Arguments.Skip(1).Select(arg => rlv.Resolve(arg)).Select(arg => ((ConstantExpression)arg).Value).ToArray();

            return call;
        }

        public bool FindMethod(Expression expression)
        {
            _args = null;
            this.Visit(expression);

            return this.Args != null;
        }
    }
}