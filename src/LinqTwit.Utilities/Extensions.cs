using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqTwit.Utilities
{
    public static class Extensions
    {
        public static bool IsProperty<T>(this MemberInfo info, Expression<Func<T, object>> expr)
        {
            string propName = expr.PropertyName();
            return propName != null && info.MemberType == MemberTypes.Property && info.Name == propName;
        }

        public static string PropertyName<T, TRet>(
            this Expression<Func<T, TRet>> expr)
        {
            if (expr.Body.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression) expr.Body).Member.Name;
            }
            return null;
        }

        public static void OnPropertyChanged<TObject, TRet>(this TObject obj,
            Expression<Func<TObject, TRet>> expr) where TObject : IRaisePropertyChanged
        {
            string propertyName = expr.PropertyName();
            if (propertyName != null)
            {
                obj.RaisePropertyChanged(propertyName);
            }
        }
    }
}