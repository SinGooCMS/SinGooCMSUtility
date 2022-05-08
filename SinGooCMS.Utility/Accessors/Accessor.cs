using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// ���ڹ���<see cref="ObjectAccessor"/>��
    /// <see cref="GenericAccessor{TInstance}"/>ʵ����
    /// </summary>    
    public abstract class Accessor
    {
        /// <summary>
        /// �û�����<see cref="Accessor"/>���͵�ʵ��
        /// </summary>
        /// <param name="type">
        /// Ҫ���з����ģ������
        /// </param>
        /// <param name="ignoreCase">
        /// ����ģ������ʱ�Ƿ���Դ�Сд
        /// </param>
        /// <param name="includeNonPublic">
        /// �Ƿ���ģ�͵ķ�<c>public</c>����
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
        /// ��ȡ����ģ������ʱ�õ������ƱȽ��� <see cref="StringComparer"/>
        /// </summary>
        protected StringComparer Comparer { get; }

        /// <summary>
        /// ��ǰ����Ķ�������
        /// </summary>
        public IReflect Type { get; }

        /// <summary>
        /// ��ȡ�������Ƿ��Ǻ��Դ�С��
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// ��ȡ�������Ƿ������public���͵�
        /// </summary>
        public bool IncludesNonPublic { get; }

        /// <summary>
        /// �����ܻ�ȡ����������Ϣ
        /// </summary>
        public SortedList<string, PropertyInfo> Properties { get; }

        /// <summary>
        /// ���ݸ����Ķ��󴴽�<see cref="ObjectAccessor"/>ʵ��
        /// </summary>
        /// <param name="instance">�����Ķ���</param>
        /// <param name="ignoreCase">�Ƿ���Դ�Сд</param>
        /// <param name="includeNonPublic">�Ƿ������public����</param>
        [DebuggerStepThrough]
        public static ObjectAccessor Build(object instance, bool ignoreCase = false, bool includeNonPublic = false)
        {
            return Build(instance.GetType(), ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// ���ݸ����Ķ������ʹ���<see cref="ObjectAccessor"/>ʵ��
        /// </summary>
        /// <param name="instanceType">�����Ķ�������</param>
        /// <param name="ignoreCase">�Ƿ���Դ�Сд</param>
        /// <param name="includeNonPublic">�Ƿ������public����</param>
        [DebuggerStepThrough]
        public static ObjectAccessor Build(Type instanceType, bool ignoreCase = false, bool includeNonPublic = false)
        {
            return new ObjectAccessor(instanceType, ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// ���ݸ����Ķ������ʹ���<see cref="GenericAccessor{TInstance}"/>ʵ��
        /// </summary>
        /// <typeparam name="TInstance">��������</typeparam>
        /// <param name="ignoreCase">�Ƿ���Դ�Сд</param>
        /// <param name="includeNonPublic">�Ƿ������public����</param>
        [DebuggerStepThrough]
        public static GenericAccessor<TInstance> Build<TInstance>(
            bool ignoreCase = false, bool includeNonPublic = false) where TInstance : class
                => new GenericAccessor<TInstance>(ignoreCase, includeNonPublic);
    }
}