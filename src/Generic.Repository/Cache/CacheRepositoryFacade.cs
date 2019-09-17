using Generic.Repository.Extension.Validation;
using System;
using System.Collections.Generic;
using System.Reflection;
using Generic.Repository.Extension.Error;
using Generic.Repository.Extension.Validation;

namespace Generic.Repository.Cache
{
    internal class CacheRepositoryFacade : ICacheRepositoryFacade
    {
        public Func<object, object> CreateFunction<TValue>(PropertyInfo property)
        {
            property.IsNull();
            var getter = property.GetGetMethod(true);
            getter.IsNull();

            return (Func<object, object>)ExtractMethod<TValue>(getter, property, "CreateFunctionGeneric");
        }

        public Func<object, object> CreateFunctionGeneric<TValue, TReturn>(MethodInfo getter)
        {
            Func<TValue, TReturn> getterTypedDelegate = (Func<TValue, TReturn>)Delegate.CreateDelegate(typeof(Func<TValue, TReturn>), getter);
            Func<object, object> getterDelegate = ((object instance) => getterTypedDelegate((TValue)instance));
            return getterDelegate;
        }

        public Action<object, object> CreateAction<TValue>(PropertyInfo property)
        {
            property.ThrowErrorNullValue(nameof(property), nameof(ExtractMethod));
            var setter = property.GetSetMethod(true);
            setter.ThrowErrorNullValue(nameof(setter), nameof(ExtractMethod)); ;

            var result = (Action<object, object>) ExtractMethod<TValue>(setter, property, "CreateActionGeneric");

            return result;
        }

        public Action<object, object> CreateActionGeneric<TValue, TInput>(MethodInfo setter)
        {
            Action<TValue, TInput> setterTypedDelegate = (Action<TValue, TInput>)Delegate.CreateDelegate(typeof(Action<TValue, TInput>), setter);
            Action<object, object> setterDelegate = (object instance, object value) => setterTypedDelegate((TValue)instance, (TInput)value);
            return setterDelegate;
        }

        private object ExtractMethod<TValue>(MethodInfo method, PropertyInfo property, string nameMethod)
        {
            method.ThrowErrorNullValue(nameof(method), nameof(ExtractMethod));
            var type = typeof(ICacheRepositoryFacade);
            var genericMethod = type.GetMethod(nameMethod);
            var genericHelper = genericMethod.MakeGenericMethod(typeof(TValue), property.PropertyType);

            return genericHelper.Invoke(this, new object[] { method });
        }

        public R GetData<R>(IDictionary<string, R> dictionary, string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException($"FIELD> {nameof(key)} VALUE> {key} METHOD> {nameof(GetData)}");
            }
            if (dictionary.TryGetValue(key, out var result))
            {
                return result;
            }
            throw new KeyNotFoundException($"FIELD> {nameof(key)} VALUE> {key} METHOD> {nameof(GetData)}");
        }
    }
}
