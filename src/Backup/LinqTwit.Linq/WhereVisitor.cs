using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqTwit.Linq
{
    class WhereVisitor : ExpressionVisitor
    {
        private MethodCallExpression where;

        public MethodCallExpression FindWhere(Expression expression)
        {
            this.Visit(expression);

            return this.where;
            
        }

        protected override Expression VisitCall(MethodCallExpression expr)
        {
            if (expr.Method.Name == "Where")
            {
                this.where = expr;
            }
            return base.VisitCall(expr);
        }

       
    }
}
