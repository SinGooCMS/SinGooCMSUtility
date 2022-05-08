using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 用于构建<see cref="ObjectAccessor"/>和
    /// <see cref="GenericAccessor{TInstance}"/>实例。
    /// </summary>    
    public abstract class Accessor
    {
        /// <summary>
        /// 用户创建<see cref="Accessor"/>类型的实例
        /// </summary>
        /// <param name="type">
        /// 要进行反射的模型类型
        /// </param>
        /// <param name="ignoreCase">
        /// 反射模型属性时是否忽略大小写
        /// </param>
        /// <param name="includeNonPublic">
        /// 是否反射模型的非<c>public</c>方法
        /// </param>
        protected Accessor(IReflect type, bool ignoreCase, bool includeNonPublic)
        {
            Type = type;
            IgnoreCase = ignoreCase;
            IncludesNonPublic = includeNonPublic;

            Comparer = IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (IncludesNonPublic)
            {
                flags = flags | BindingFlags.NonPublic;
            }

            var props = Type.GetProperties(flags);
            Properties = new SortedList<string, PropertyInfo>(props.Length, Comparer);
            foreach (var prop in props)
            {
                Properties[prop.Name] = prop;
            }
        }

        /// <summary>
        /// 获取反射模型属性时用到的名称比较器 <see cref="StringComparer"/>
        /// </summary>
        protected StringComparer Comparer { get; }

        /// <summary>
        /// 当前反射的对象类型
        /// </summary>
        public IReflect Type { get; }

        /// <summary>
        /// 获取的属性是否是忽略大小的
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// 获取的属性是否包含非public类型的
        /// </summary>
        public bool IncludesNonPublic { get; }

        /// <summary>
        /// 所有能获取到的属性信息
        /// </summary>
        public SortedList<string, PropertyInfo> Properties { get; }

        /// <summary>
        /// 根据给定的对象创建<see cref="ObjectAccessor"/>实例
        /// </summary>
        /// <param name="instance">给定的对象</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="includeNonPublic">是否包含非public属性</param>
        [DebuggerStepThrough]
        public static ObjectAccessor Build(object instance, bool ignoreCase = false, bool includeNonPublic = false)
        {
            return Build(instance.GetType(), ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// 根据给定的对象类型创建<see cref="ObjectAccessor"/>实例
        /// </summary>
        /// <param name="instanceType">给定的对象类型</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="includeNonPublic">是否包含非public属性</param>
        [DebuggerStepThrough]
        public static ObjectAccessor Build(Type instanceType, bool ignoreCase = false, bool includeNonPublic = false)
        {
            return new ObjectAccessor(instanceType, ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// 根据给定的对象类型创建<see cref="GenericAccessor{TInstance}"/>实例
        /// </summary>
        /// <typeparam name="TInstance">对象类型</typeparam>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="includeNonPublic">是否包含非public属性</param>
        [DebuggerStepThrough]
        public static GenericAccessor<TInstance> Build<TInstance>(
            bool ignoreCase = false, bool includeNonPublic = false) where TInstance : class
                => new GenericAccessor<TInstance>(ignoreCase, includeNonPublic);
    }
}