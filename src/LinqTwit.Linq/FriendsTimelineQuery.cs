﻿using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqTwit.Linq.Impl;
using LinqTwit.Twitter;
using LinqTwit.Utilities;

namespace LinqTwit.Linq
{
    class FriendsTimelineQuery : IQuery
    {
        private readonly ILinqApi _linqApi;

        public FriendsTimelineQuery(ILinqApi linqApi)
        {
            this._linqApi = linqApi;
        }

        public object Execute(Expression expression, bool isEnumerable)
        {
            FriendsTimeLineArgs args = new FriendsTimeLineArgs();
            HandleWhereArgs(expression, args);

            HandleTakeArgs(expression, args);

            HandlePageArgs(expression, args);

            return _linqApi.FriendsTimeLine(args).Select(t => new Tweet(t)).Cast<ITweet>(); 
        }

        private void HandlePageArgs(Expression expression, FriendsTimeLineArgs args)
        {
            MethodInfo info = MethodInfoForPage();
            MethodCallFinderVisitor visitor = new MethodCallFinderVisitor(info);

            if (visitor.FindMethod(expression))
            {
                args.Page = Convert.ToInt32(visitor.Args.First());
            }
        }

        private static MethodInfo MethodInfoForPage()
        {
            IQueryable<ITweet> queryable;
            return
                Extensions.MethodOf<IQueryable<ITweet>, IQueryable<ITweet>>(
                    q => q.Page(42));
        }

        private static void HandleTakeArgs(Expression expression, FriendsTimeLineArgs args)
        {
            MethodInfo info = MethodInfoForTake();
            MethodCallFinderVisitor visitor = new MethodCallFinderVisitor(info);

            if (visitor.FindMethod(expression))
            {
                args.Count = Convert.ToInt32(visitor.Args.First());
            }
        }

        private static MethodInfo MethodInfoForTake()
        {
            IQueryable<ITweet> queryable;
            return Extensions.MethodOf<IQueryable<ITweet>, IQueryable<ITweet>>(q => q.Take(10));
        }

        private void HandleWhereArgs(Expression expression, FriendsTimeLineArgs args)
        {
            var whereVisitor = new WhereVisitor();

            MethodCallExpression whereExpression = whereVisitor.FindWhere(expression);
            if (whereExpression == null)
            {
                return;
            }

            LambdaExpression lambdaExpression =
                (LambdaExpression)
                ((UnaryExpression)whereExpression.Arguments[1]).Operand;

            var idVisitor = new IdExpressionVisitor();
            if (!idVisitor.FindIdExpression(lambdaExpression))
            {
                return;
            }

            foreach (BinaryExpression binaryExpression in idVisitor.Expressions)
            {
                BuildArgs(binaryExpression, args);
            }
        }

        private static void BuildArgs(BinaryExpression expression, FriendsTimeLineArgs args)
        {
            if (expression.Left.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotSupportedException(string.Format("Expression {0} not supported", expression.Left));
            }

            MemberExpression me = (MemberExpression) expression.Left;
            if (!me.Member.IsProperty<ITweet, long>(t => t.Id))
            {
                throw new NotSupportedException(String.Format("Expression {0} not supported", me));
            }

            long id = ((IConvertible)((ConstantExpression)expression.Right).Value).ToInt64(CultureInfo.InvariantCulture);

            switch (expression.NodeType)
            {
                case ExpressionType.GreaterThanOrEqual:
                    args.SinceId = id;
                    break;
                case ExpressionType.LessThanOrEqual:
                    args.MaxId = id;
                    break;
                default:
                    throw new NotSupportedException(
                        String.Format("Expression of type {0} not supported", expression.NodeType));
            }
        }
    }
}