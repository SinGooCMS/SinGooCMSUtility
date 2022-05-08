using SinGooCMS.Utility.Extension;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 提供一系列快速构建<seealso cref="Accessor"/>实例的方法
    /// </summary>
    public static class AccessorBuilder
    {
        /// <summary>
        /// 根据给定的属性名称采获取属性的写入逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyName">属性名称</param>
        public static Action<TInstance, TProperty> BuildSetter<TInstance, TProperty>(string propertyName, bool includePrivate = false) where TInstance : class
        {
            if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo propInfo))
            {
                throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
            }
            return BuildSetter<TInstance, TProperty>(propInfo, includePrivate);
        }

        /// <summary>
        /// 根据给定属性信息获取属性的写入逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyInfo">属性信息</param>
        public static Action<TInstance, TProperty> BuildSetter<TInstance, TProperty>(PropertyInfo propertyInfo, bool includePrivate = false) where TInstance : class
        {
            if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support writing.");
            }

            var setMethod = propertyInfo.GetSetMethod(includePrivate);
            return (Action<TInstance, TProperty>)Delegate.CreateDelegate(typeof(Action<TInstance, TProperty>), setMethod);
        }

        /// <summary>
        /// 根据给定属性名称获取属性的读取逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyName">属性名称</param>
        public static Func<TInstance, TProperty> BuildGetter<TInstance, TProperty>(string propertyName, bool includePrivate = false) where TInstance : class
        {
            if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo propInfo))
            {
                throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
            }
            return BuildGetter<TInstance, TProperty>(propInfo, includePrivate);
        }

        /// <summary>
        ///根据给定属性信息获取属性的读取逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyInfo">属性信息</param>
        public static Func<TInstance, TProperty> BuildGetter<TInstance, TProperty>(PropertyInfo propertyInfo, bool includePrivate = false) where TInstance : class
        {
            if (!propertyInfo.CanRead)
            {
                throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support reading.");
            }

            var getMethod = propertyInfo.GetGetMethod(includePrivate);
            return (Func<TInstance, TProperty>)Delegate.CreateDelegate(typeof(Func<TInstance, TProperty>), getMethod);
        }

        /// <summary>
        /// 根据给定属性信息获取属性的写入逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyInfo">属性信息</param>
        public static Action<object, object> BuildSetter(PropertyInfo propertyInfo, bool includePrivate = false)
        {
            var instanceType = propertyInfo.ReflectedType;

            if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{instanceType?.FullName}` does not support writing.");
            }

            var setMethod = propertyInfo.GetSetMethod(includePrivate);
            var typeofObject = typeof(object);

            var instance = Expression.Parameter(typeofObject, "instance");
            var value = Expression.Parameter(typeofObject, "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            var instanceCast = !instanceType.GetTypeInfo().IsValueType
                ? Expression.TypeAs(instance, instanceType)
                : Expression.Convert(instance, instanceType);

            var valueCast = !propertyInfo.PropertyType.GetTypeInfo().IsValueType
                ? Expression.TypeAs(value, propertyInfo.PropertyType)
                : Expression.Convert(value, propertyInfo.PropertyType);

            return Expression.Lambda<Action<object, object>>(
                Expression.Call(instanceCast, setMethod, valueCast), instance, value).Compile();
        }

        /// <summary>
        ///根据给定属性信息获取属性的读取逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyInfo">属性信息</param>
        public static Func<object, object> BuildGetter(PropertyInfo propertyInfo, bool includePrivate = false)
        {
            var instanceType = propertyInfo.ReflectedType;
            if (!propertyInfo.CanRead)
            {
                throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{instanceType?.FullName}` does not support reading.");
            }

            var getMethod = propertyInfo.GetGetMethod(includePrivate);
            var typeofObject = typeof(object);

            var instance = Expression.Parameter(typeofObject, "instance");
            var isValueType = instanceType.GetTypeInfo().IsValueType;
            var instanceCast = !isValueType
                ? Expression.TypeAs(instance, instanceType)
                : Expression.Convert(instance, instanceType);

            return Expression.Lambda<Func<object, object>>(
                Expression.TypeAs(Expression.Call(instanceCast, getMethod), typeofObject), instance).Compile();
        }

        /// <summary>
        ///根据给定属性名称获取属性的写入逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyName">属性名称</param>
        public static Action<TInstance, object> BuildSetter<TInstance>(string propertyName, bool includePrivate = false) where TInstance : class
        {
            if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo propInfo))
            {
                throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
            }

            return BuildSetter<TInstance>(propInfo, includePrivate);
        }

        /// <summary>
        ///根据给定属性信息获取属性的写入逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyInfo">属性信息</param>
        public static Action<TInstance, object> BuildSetter<TInstance>(PropertyInfo propertyInfo, bool includePrivate = false) where TInstance : class
        {
            if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support writing.");
            }

            var setMethod = propertyInfo.GetSetMethod(includePrivate);

            var instance = Expression.Parameter(typeof(TInstance), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var isValueType = propertyInfo.PropertyType.GetTypeInfo().IsValueType;
            var valueCast = !isValueType
                ? Expression.TypeAs(value, propertyInfo.PropertyType)
                : Expression.Convert(value, propertyInfo.PropertyType);

            return Expression.Lambda<Action<TInstance, object>>(
                Expression.Call(instance, setMethod, valueCast), instance, value).Compile();
        }

        /// <summary>
        ///根据给定属性名称获取属性的读取逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyName">属性名称</param>
        public static Func<TInstance, object> BuildGetter<TInstance>(string propertyName, bool includePrivate = false)
        {
            if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo propInfo))
            {
                throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
            }

            return BuildGetter<TInstance>(propInfo, includePrivate);
        }

        /// <summary>
        ///根据给定属性信息获取属性的写入逻辑
        /// </summary>
        /// <param name="includePrivate">是否排除private方法</param>
        /// <param name="propertyInfo">属性信息</param>
        public static Func<TInstance, object> BuildGetter<TInstance>(PropertyInfo propertyInfo, bool includePrivate = false)
        {
            if (!propertyInfo.CanRead)
            {
                throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support reading.");
            }

            var getMethod = propertyInfo.GetGetMethod(includePrivate);

            var instance = Expression.Parameter(typeof(TInstance), "instance");
            return Expression.Lambda<Func<TInstance, object>>(
                Expression.TypeAs(Expression.Call(instance, getMethod), typeof(object)), instance).Compile();
        }

        /// <summary>
        ///根据给定对象类型获取构造函数
        /// </summary>
        public static Func<TInstance> BuildInstanceCreator<TInstance>() where TInstance : new()
        {
            var type = typeof(TInstance);

            var dynamicMethod = new DynamicMethod("Build_" + type.Name, type, new Type[0], typeof(AccessorBuilder).Module, true);
            var ilGen = dynamicMethod.GetILGenerator();

            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            if (defaultCtor != null)
            {
                // Call the ctor, all values on the stack are passed to the ctor
                ilGen.Emit(OpCodes.Newobj, defaultCtor);
            }
            else
            {
                var builder = ilGen.DeclareLocal(type);
                ilGen.Emit(OpCodes.Ldloca, builder);
                ilGen.Emit(OpCodes.Initobj, type);
                ilGen.Emit(OpCodes.Ldloc, builder);
            }

            // Return the new object
            ilGen.Emit(OpCodes.Ret);

            return (Func<TInstance>)dynamicMethod.CreateDelegate(typeof(Func<TInstance>));
        }

        /// <summary>
        /// 根据对象的构造函数信息创建对象构建逻辑
        /// <exception cref="IndexOutOfRangeException">
        /// 如果参数数量不正确
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// 如果参数类型不正确
        /// </exception>
        /// </summary>
        public static Func<object[], TInstance> BuildInstanceCreator<TInstance>(ConstructorInfo constructor)
        {
            var type = typeof(TInstance);

            var ctroParams = constructor.GetParameters();

            var dynamicMethod = new DynamicMethod("Build_" + type.Name, type, new[] { typeof(object[]) }, typeof(AccessorBuilder).Module, true);
            var ilGen = dynamicMethod.GetILGenerator();

            // Cast each argument of the input object array to the appropriate type.
            for (var i = 0; i < ctroParams.Length; i++)
            {
                ilGen.Emit(OpCodes.Ldarg_0); // Push Object array
                ilGen.Emit(OpCodes.Ldc_I4, i); // Push the index to access
                ilGen.Emit(OpCodes.Ldelem_Ref); // Push the element at the previously loaded index

                // Cast the object to the appropriate ctor Parameter Type
                var paramType = ctroParams[i].ParameterType;
                var isValueType = paramType.GetTypeInfo().IsValueType;
                ilGen.Emit(isValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
            }

            // Call the ctor, all values on the stack are passed to the ctor
            ilGen.Emit(OpCodes.Newobj, constructor);
            // Return the new object
            ilGen.Emit(OpCodes.Ret);

            return (Func<object[], TInstance>)dynamicMethod.CreateDelegate(typeof(Func<object[], TInstance>));
        }
    }
}