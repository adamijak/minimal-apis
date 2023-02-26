using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace WebApi.Helpers;

public static class QueryParameterHelpers
{
    public static Func<Tin, object> GetSelectFunction<Tin>(params string[] selectParameters)
    {
        return new Func<Tin, object>(obj =>
        {
            var type = obj.GetType();
            var d = new ExpandoObject() as IDictionary<string, object>;
            foreach (var sel in selectParameters)
            {
                d.Add(sel, type.GetProperty(sel).GetValue(obj));
            }
            return d;
        });
    }

    public static Func<T, T> GetCompiledSelectFunction<T>(params string[] propertyNames)
    {
        var type = typeof(T);
        // input parameter "o"
        var xParameter = Expression.Parameter(type, "o");

        // new statement "new Data()"
        var xNew = Expression.New(type);

        // create initializers
        var bindings = propertyNames.Select(p =>
        {
            var property = type.GetProperty(p);
            // original value "o.Field1"
            var xOriginal = Expression.Property(xParameter, property);

            // set value "Field1 = o.Field1"
            return Expression.Bind(property, xOriginal);
        });
        // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var xInit = Expression.MemberInit(xNew, bindings);

        // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

        // compile to Func<Data, Data>
        return lambda.Compile();
    }
}

