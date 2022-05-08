using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 快速访问给定类型的对象的属性值, 参照: <seealso cref="Accessor"/>
    /// </summary>
    public class ObjectAccessor : Accessor
    {
        private readonly Hashtable _objectGettersCache, _objectSettersCache;

        [DebuggerStepThrough]
        internal ObjectAccessor(IReflect type, bool ignoreCase, bool includeNonPublic)
            : base(type, ignoreCase, includeNonPublic)
        {
            _objectGettersCache = new Hashtable(Properties.Count, Comparer);
            _objectSettersCache = new Hashtable(Properties.Count, Comparer);

            foreach (var pair in Properties)
            {
                var propName = pair.Key;
                var prop = pair.Value;

                if (prop.CanRead)
                {
                    _objectGettersCache[propName] = AccessorBuilder.BuildGetter(prop, IncludesNonPublic);
                }

                if (prop.CanWrite)
                {
                    _objectSettersCache[propName] = AccessorBuilder.BuildSetter(prop, IncludesNonPublic);
                }
            }
        }

        /// <summary>
        /// 根据给定的对象和属性名称对属性进行读写操作
        /// </summary>
        public object this[object instance, string propertyName]
        {
            get
            {
                if (_objectGettersCache[propertyName] is Func<object, object> getter)
                {
                    return getter(instance);
                }
                throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports reading.");
            }

            set
            {
                if (_objectSettersCache[propertyName] is Action<object, object> setter)
                {
                    setter(instance, value);
                }
                else
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports writing.");
                }
            }
        }
    }
}