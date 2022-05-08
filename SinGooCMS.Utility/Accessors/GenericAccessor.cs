using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 快速访问给定类型的对象的属性值, 参照: <seealso cref="Accessor"/>
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public sealed class GenericAccessor<TInstance> : ObjectAccessor where TInstance : class
    {
        private readonly Hashtable
            _genericInstanceGettersCache,
            _genericInstanceSettersCache,
            _genericPropertiesGettersCache,
            _genericPropertiesSettersCache;

        [DebuggerStepThrough]
        internal GenericAccessor(bool ignoreCase, bool includeNonPublic)
            : base(typeof(TInstance), ignoreCase, includeNonPublic)
        {
            _genericPropertiesGettersCache = new Hashtable(Properties.Count, Comparer);
            _genericPropertiesSettersCache = new Hashtable(Properties.Count, Comparer);
            _genericInstanceGettersCache = new Hashtable(Properties.Count, Comparer);
            _genericInstanceSettersCache = new Hashtable(Properties.Count, Comparer);

            foreach (var pair in Properties)
            {
                var propName = pair.Key;
                var prop = pair.Value;

                if (prop.CanRead)
                {
                    _genericPropertiesGettersCache[propName] = AccessorBuilder.BuildGetter<TInstance>(prop, IncludesNonPublic);
                }

                if (prop.CanWrite)
                {
                    _genericPropertiesSettersCache[propName] = AccessorBuilder.BuildSetter<TInstance>(prop, IncludesNonPublic);
                }
            }
        }

        /// <summary>
        /// 根据给定的对象和属性名称对属性进行读写操作
        /// </summary>
        public object this[TInstance instance, string propertyName]
        {
            get
            {
                if (_genericPropertiesGettersCache[propertyName] is Func<TInstance, object> getter)
                {
                    return getter(instance);
                }
                throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports reading.");
            }

            set
            {
                if (_genericPropertiesSettersCache[propertyName] is Action<TInstance, object> setter)
                {
                    setter(instance, value);
                }
                else
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports writing.");
                }
            }
        }

        /// <summary>
        /// 尝试获取指定对象的指定属性值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<TProperty>(TInstance instance, string propertyName, out TProperty value)
        {
            var cache = _genericInstanceGettersCache;

            var getter = cache[propertyName] as Func<TInstance, TProperty>;
            if (getter is null)
            {
                if (!Properties.TryGetValue(propertyName, out var prop))
                {
                    value = default;
                    return false;
                }

                lock (cache)
                {
                    getter = cache[propertyName] as Func<TInstance, TProperty>;
                    if (getter is null)
                    {
                        try
                        {
                            getter = AccessorBuilder.BuildGetter<TInstance, TProperty>(
                                prop.Name, IncludesNonPublic);
                            cache[prop.Name] = getter;
                        }
                        catch (ArgumentException)
                        {
                            value = default;
                            return false;
                        }
                    }
                }
            }

            value = getter(instance);
            return true;
        }

        /// <summary>
        /// 尝试设置指定对象的指定属性值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySet<TProperty>(TInstance instance, string propertyName, TProperty value)
        {
            var cache = _genericInstanceSettersCache;

            var setter = cache[propertyName] as Action<TInstance, TProperty>;
            if (setter is null)
            {
                if (!Properties.TryGetValue(propertyName, out var prop)) { return false; }

                lock (cache)
                {
                    setter = cache[propertyName] as Action<TInstance, TProperty>;
                    if (setter is null)
                    {
                        try
                        {
                            setter = AccessorBuilder.BuildSetter<TInstance, TProperty>(
                                prop.Name, IncludesNonPublic);
                            cache[prop.Name] = setter;
                        }
                        catch (ArgumentException)
                        {
                            return false;
                        }
                    }
                }
            }

            setter(instance, value);
            return true;
        }
    }
}