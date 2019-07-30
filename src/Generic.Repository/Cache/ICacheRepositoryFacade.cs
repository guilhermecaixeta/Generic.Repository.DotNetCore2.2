using System;
using System.Collections.Generic;
using System.Reflection;

namespace Generic.Repository.Cache
{
    internal interface ICacheRepositoryFacade
    {
        Func<object, object> CreateFunction<TValue>(PropertyInfo property);

        Func<object, object> CreateFunctionGeneric<TValue, TReturn>(MethodInfo getter);

        Action<object, object> CreateAction<TValue>(PropertyInfo property);

        Action<object, object> CreateActionGeneric<TValue, TInput>(MethodInfo setter);

        R GetData<R>(IDictionary<string, R> dictionary, string key);
    }
}