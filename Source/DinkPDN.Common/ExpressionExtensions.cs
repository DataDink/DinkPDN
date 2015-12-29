using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static MemberInfo GetMember(this Expression expression)
        {
            while (expression is UnaryExpression) expression = ((UnaryExpression)expression).Operand;
            if (!(expression is MemberExpression)) throw new ArgumentException("Member expression expected (t => t.Member)");
            return ((MemberExpression)expression).Member;
        }

        public static MemberInfo GetMember<T>(this Expression<Func<T, object>> expression) { return expression.Body.GetMember(); }

        public static PropertyInfo GetProperty(this Expression expression) { return expression.GetMember() as PropertyInfo; }

        public static PropertyInfo GetProperty<T>(this Expression<Func<T, object>> expression) { return expression.Body.GetProperty(); }

        public static FieldInfo GetField(this Expression expression) { return expression.GetMember() as FieldInfo; }

        public static FieldInfo GetField<T>(this Expression<Func<T, object>> expression) { return expression.Body.GetField(); }

        public static MemberInfo[] GetMembers(this IEnumerable<Expression> expressions) { return expressions.Select(e => e.GetMember()).ToArray(); }

        public static MemberInfo[] GetMembers<T>(this IEnumerable<Expression<Func<T, object>>> expressions) { return expressions.Select(e => e.Body.GetMember()).ToArray(); }

        public static PropertyInfo[] GetProperties(this IEnumerable<Expression> expressions) { return expressions.Select(e => e.GetMember() as PropertyInfo).ToArray(); }

        public static PropertyInfo[] GetProperties<T>(this IEnumerable<Expression<Func<T, object>>> expressions) { return expressions.Select(e => e.Body.GetMember() as PropertyInfo).ToArray(); }

        public static FieldInfo[] GetFields(this IEnumerable<Expression> expressions) { return expressions.Select(e => e.GetMember() as FieldInfo).ToArray(); }

        public static FieldInfo[] GetFields<T>(this IEnumerable<Expression<Func<T, object>>> expressions) { return expressions.Select(e => e.Body.GetMember() as FieldInfo).ToArray(); }

    }
}
