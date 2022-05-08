using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// <see cref="Type"/>扩展类
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Type[] SimpleTypes =
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(string),
            typeof(char),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(byte[])
        };

        private static readonly Dictionary<string, SequenceType> NonGenericCollectionsToSequenceTypeMapping = new Dictionary<string, SequenceType>(StringComparer.Ordinal)
        {
            { "System.String", SequenceType.String },
            { "System.Collections.ArrayList", SequenceType.ArrayList },
            { "System.Collections.Queue", SequenceType.Queue },
            { "System.Collections.Stack", SequenceType.Stack },
            { "System.Collections.BitArray", SequenceType.BitArray },
            { "System.Collections.SortedList", SequenceType.SortedList },
            { "System.Collections.Hashtable", SequenceType.Hashtable },
            { "System.Collections.Specialized.ListDictionary", SequenceType.ListDictionary },
            { "System.Collections.IList", SequenceType.IList },
            { "System.Collections.ICollection", SequenceType.ICollection },
            { "System.Collections.IDictionary", SequenceType.IDictionary },
            { "System.Collections.IEnumerable", SequenceType.IEnumerable }
        };

        /// <summary>
        /// 尝试反射返回给定<paramref name="type"/>的<c>实例</c>属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="property">返回的属性</param>
        /// <param name="inherit">是否返回继承属性</param>
        /// <returns></returns>
        public static bool TryGetInstanceProperty(this Type type, string propertyName, out PropertyInfo property, bool inherit = true)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            if (!inherit) { flags = flags | BindingFlags.DeclaredOnly; }

            property = type.GetProperties(flags)
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.Ordinal));

            return property != null;
        }

        /// <summary>
        /// 尝试反射返回给定<paramref name="type"/>的<c>实例</c>属性
        /// </summary>
        /// <remarks>此方法可用于返回<c>public</c>或<c>non-public </c>属性。</remarks>
        /// <param name="type"></param>
        /// <param name="inherit">是否返回继承属性</param>
        /// <param name="includePrivate">是否包含私有属性</param>
        /// <returns></returns>
        public static PropertyInfo[] GetInstanceProperties(this Type type, bool inherit = true, bool includePrivate = true)
        {
            return GetInstanceProperties(type.GetTypeInfo(), inherit, includePrivate);
        }

        /// <summary>
        /// 根据条件返回所有能获取到的属性描述
        /// </summary>
        /// <remarks>此方法可用于返回<c>public</c>或<c>non-public </c>属性。</remarks>
        /// <param name="typeInfo"></param>
        /// <param name="inherit">是否返回继承属性</param>
        /// <param name="includePrivate">是否包含私有属性</param>
        public static PropertyInfo[] GetInstanceProperties(this TypeInfo typeInfo, bool inherit, bool includePrivate)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            if (includePrivate) { flags = flags | BindingFlags.NonPublic; }
            if (!inherit) { flags = flags | BindingFlags.DeclaredOnly; }

            return typeInfo.GetProperties(flags);
        }

        /// <summary>
        /// 根据条件返回具有 <c>T</c> 特性的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit">是否返回继承的属性</param>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return type.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(T)) && (prop.DeclaringType == type || inherit));
        }

        /// <summary>
        /// 返回具有 <c>T</c> 特性的属性, 并将 <c>T</c> 实例和对应的属性描述转为字典
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit">是否包含继承的属性</param>
        public static Dictionary<T, PropertyInfo> GetAttributeToPropertyMapping<T>(this Type type, bool inherit = true) where T : Attribute
        {
            var properties = type.GetProperties();
            var result = new Dictionary<T, PropertyInfo>(properties.Length);
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<T>(inherit);
                var attr = attributes.FirstOrDefault();
                if (attr is null) { continue; }

                result[attr] = property;
            }
            return result;
        }

        /// <summary>
        /// 尝试获取当前 <seealso cref="Type"/> 上定义的所有 <typeparamref name="T"/> 类型的特性数组
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributes"></param>
        /// <param name="inherit">是否包含继承的特性</param>
        public static bool TryGetAttributes<T>(this Type type, out T[] attributes, bool inherit = true) where T : Attribute
        {
            var result = Attribute.GetCustomAttributes(type, typeof(T), inherit);

            if (result.Length > 0)
            {
                attributes = result as T[];
                return true;
            }

            attributes = null;
            return false;
        }

        /// <summary>
        /// 如果当前类型具有泛型参数的话,就返回所有的泛型类型，代码示例：
        /// </summary>
        /// <example>
        /// <code>
        /// if (typeof(Dictionary&lt;string, object&gt;).TryGetGenericArguments(out Type[] res))
        /// {
        ///     //输出: System.String,System.Object
        ///     Console.WriteLine(string.Join(",", res.ToList()));
        /// }   
        /// </code>
        /// </example>
        public static bool TryGetGenericArguments(this Type type, out Type[] genericArguments)
        {
            if (type.IsArray)
            {
                genericArguments = new[] { type.GetElementType() };
                return true;
            }

            if (!type.IsGenericType)
            {
                genericArguments = null;
                return false;
            }

            genericArguments = type.GetGenericArguments();
            return true;
        }

        /// <summary>
        /// 判断当前类型是否是序列(数组、集合) 注意：包括字符串
        /// </summary>
        public static bool IsSequence(this Type type, out SequenceType sequenceType)
        {
            if (type.IsArray)
            {
                sequenceType = SequenceType.Array;
                return true;
            }

            if (NonGenericCollectionsToSequenceTypeMapping.TryGetValue(type.FullName, out sequenceType))
            {
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.List`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.HashSet`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericHashSet;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.ObjectModel.Collection`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericCollection;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.LinkedList`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericLinkedList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.Stack`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericStack;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.Queue`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericQueue;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.ICollection`1[[System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericICollectionKeyValue;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.ICollection`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericICollection;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIEnumerableKeyValue;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIEnumerable;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.Dictionary`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.SortedDictionary`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericSortedDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.SortedList`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericSortedList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IDictionary`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.ICollection`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Concurrent.BlockingCollection`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericBlockingCollection;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Concurrent.ConcurrentBag`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericConcurrentBag;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Concurrent.ConcurrentDictionary`2[[", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericConcurrentDictionary;
                return true;
            }

            var interfaces = type.GetInterfaces().ToArray();

            if (interfaces.Any(i => i.Name.StartsWith("IEnumerable`1", StringComparison.Ordinal)))
            {
                sequenceType = SequenceType.GenericCustom;
                return true;
            }

            if (interfaces.Any(i => i.Name.StartsWith("IEnumerable", StringComparison.Ordinal)))
            {
                sequenceType = SequenceType.Custom;
                return true;
            }

            sequenceType = SequenceType.Invalid;
            return false;
        }

        /// <summary>
        /// 判断当前类型是否继承自<typeparamref name="T"/> 注意：<typeparamref name="T"/>可以是接口或类
        /// </summary>
        public static bool IsImplements<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// 判断当前类型是否具有空参数构造函数
        /// </summary>
        public static bool HasDefaultConstructor(this Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// 判断当前类型是否是简单类型
        /// </summary>
        /// <remarks>
        /// <para>
        /// 简单类型如下（包含简单类型的可空类型）
        /// </para>
        /// <code>
        /// typeof(byte),
        /// typeof(sbyte),
        /// typeof(short),
        /// typeof(ushort),
        /// typeof(int),
        /// typeof(uint),
        /// typeof(long),
        /// typeof(ulong),
        /// typeof(float),
        /// typeof(double),
        /// typeof(decimal),
        /// typeof(bool),
        /// typeof(string),
        /// typeof(char),
        /// typeof(Guid),
        /// typeof(DateTime),
        /// typeof(DateTimeOffset),
        /// typeof(TimeSpan),
        /// typeof(byte[])
        /// </code>
        /// </remarks>
        public static bool IsSimpleType(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;

            return Array.IndexOf(SimpleTypes, type) > -1 || type.IsEnum;
        }

        /// <summary>
        /// 判断当前类型是否是数组 <c>T[]</c>
        /// </summary>
        public static bool IsArrayOf<T>(this Type type) => type == typeof(T[]);

        /// <summary>
        /// 判断当前类型是否是 <c>List&lt;&gt;</c> 或 <c>IList&lt;&gt;</c>
        /// </summary>
        public static bool IsGenericList(this Type type)
        {
            if (!type.IsGenericType) { return false; }

            var typeDef = type.GetGenericTypeDefinition();

            if (typeDef == typeof(List<>) || typeDef == typeof(IList<>)) { return true; }

            return false;
        }

        /// <summary>
        /// 判断当前类型是否是数字类型(如: "int","int?" 均是数字类型 )
        /// </summary>
        public static bool IsNumeric(this Type type)
        {
            if (type is null) { return false; }

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            if (underlyingType.GetTypeInfo().IsEnum) { return false; }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (underlyingType.GetTypeCode())
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 获取当前类型的<seealso cref="TypeCode"/>
        /// </summary>
        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == typeof(bool)) { return TypeCode.Boolean; }
            if (type == typeof(char)) { return TypeCode.Char; }
            if (type == typeof(sbyte)) { return TypeCode.SByte; }
            if (type == typeof(byte)) { return TypeCode.Byte; }
            if (type == typeof(short)) { return TypeCode.Int16; }
            if (type == typeof(ushort)) { return TypeCode.UInt16; }
            if (type == typeof(int)) { return TypeCode.Int32; }
            if (type == typeof(uint)) { return TypeCode.UInt32; }
            if (type == typeof(long)) { return TypeCode.Int64; }
            if (type == typeof(ulong)) { return TypeCode.UInt64; }
            if (type == typeof(float)) { return TypeCode.Single; }
            if (type == typeof(double)) { return TypeCode.Double; }
            if (type == typeof(decimal)) { return TypeCode.Decimal; }
            if (type == typeof(DateTime)) { return TypeCode.DateTime; }
            if (type == typeof(string)) { return TypeCode.String; }
            // ReSharper disable once TailRecursiveCall
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (type.GetTypeInfo().IsEnum) { return Enum.GetUnderlyingType(type).GetTypeCode(); }
            return TypeCode.Object;
        }

        /// <summary>
        /// 获取当前类型的名称,如: 
        /// <list type="number">
        /// <item>typeof(List&lt;>) => System.Collections.Generic.List&lt;T></item>
        /// <item>typeof(List&lt;Person>) => System.Collections.Generic.List&lt;DotNetCommonTestNamespace.Person></item>
        /// <item>typeof(DemoFu&lt;int?>.DemoZi&lt;long[], double?[]>.DemoSunlt;string>) => DotNetCommonTestNamespace.DemoFu&lt;int?>.DemoZi&lt;long[],double?[]>.DemoSun&lt;string></item>
        /// </list>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClassFullName(this Type type)
        {
            return classNamesCache.GetOrAdd(type, sun =>
            {
                var types = type.GetGenericArguments().ToList();
                var parents = new List<Type>();
                //获取所有父类型
                GetParents(parents, type);
                parents.Reverse();
                //
                var names = new List<string>();
                var counter = 0;
                for (var i = 0; i < parents.Count; i++)
                {
                    var parent = parents[i];
                    var name = "";
                    if (shortNames.ContainsKey(parent))
                    {
                        name = shortNames[parent];
                        names.Add(name);
                        continue;
                    }
                    else
                    {
                        name = (i == 0 ? parent.Namespace + "." : "") + parent.Name.ToSpliterArray<string>('`').FirstOrDefault();
                    }

                    if (parent.IsGenericType)
                    {
                        var defs = parent.GetGenericTypeDefinition().GetGenericArguments();
                        var geneTypes = types.Take(defs.Length).Skip(counter)
                        .Select(p => shortNames.ContainsKey(p)
                                ? shortNames[p]
                                : (p.IsGenericParameter ? p.Name : GetClassFullName(p))
                        )
                        .ToSplitterString(",");

                        if (parent.IsNullable())
                        {
                            if (geneTypes.Length > 0) name = $"{geneTypes}?";
                        }
                        else
                        {
                            if (geneTypes.Length > 0) name += "<" + geneTypes + ">";
                        }

                        counter = defs.Length;
                    }
                    names.Add(name);
                }
                if (type.IsArray && !names.LastOrDefault().Contains("[]")) return names.ToSplitterString(".") + "[]";
                return names.ToSplitterString(".");
            });
        }
        private static void GetParents(List<Type> parents, Type type)
        {
            parents.Add(type);
            if (type.IsArray)
            {
                var tmp = type.GetElementType();
                if (tmp.DeclaringType != null) GetParents(parents, tmp.DeclaringType);
            }
            else if (type.DeclaringType != null)
            {
                GetParents(parents, type.DeclaringType);
            }
        }
        private static Dictionary<Type, string> shortNames = new Dictionary<Type, string>()
        {
            { typeof(byte),"byte"},{ typeof(byte?),"byte?"},{ typeof(byte[]),"byte[]"},{ typeof(byte?[]),"byte?[]"},
            { typeof(sbyte),"sbyte"},{ typeof(sbyte?),"sbyte?"},{ typeof(sbyte[]),"sbyte[]"},{ typeof(sbyte?[]),"sbyte?[]"},
            { typeof(short),"short"},{ typeof(short?),"short?"},{ typeof(short[]),"short[]"},{ typeof(short?[]),"short?[]"},
            { typeof(ushort),"ushort"},{ typeof(ushort?),"ushort?"},{ typeof(ushort[]),"ushort[]"},{ typeof(ushort?[]),"ushort?[]"},
            { typeof(int),"int"},{ typeof(int?),"int?"},{ typeof(int[]),"int[]"},{ typeof(int?[]),"int?[]"},
            { typeof(uint),"uint"},{ typeof(uint?),"uint?"},{ typeof(uint[]),"uint[]"},{ typeof(uint?[]),"uint?[]"},
            { typeof(long),"long"},{ typeof(long?),"long?"},{ typeof(long[]),"long[]"},{ typeof(long?[]),"long?[]"},
            { typeof(ulong),"ulong"},{ typeof(ulong?),"ulong?"},{ typeof(ulong[]),"ulong[]"},{ typeof(ulong?[]),"ulong?[]"},
            { typeof(float),"float"},{ typeof(float?),"float?"},{ typeof(float[]),"float[]"},{ typeof(float?[]),"float?[]"},
            { typeof(double),"double"},{ typeof(double?),"double?"},{ typeof(double[]),"double[]"},{ typeof(double?[]),"double?[]"},
            { typeof(decimal),"decimal"},{ typeof(decimal?),"decimal?"},{ typeof(decimal[]),"decimal[]"},{ typeof(decimal?[]),"decimal?[]"},
            { typeof(char),"char"},{ typeof(char?),"char?"},{ typeof(char[]),"char[]"},{ typeof(char?[]),"char?[]"},
            { typeof(string),"string"},{ typeof(string[]),"string[]"},
            { typeof(bool),"bool"},{ typeof(bool?),"bool?"},{ typeof(bool[]),"bool[]"},{ typeof(bool?[]),"bool?[]"},
        };
        private static ConcurrentDictionary<Type, string> classNamesCache = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// 是否是Nullable类型的
        /// </summary>
        /// <param name="type"></param>
        public static bool IsNullable(this Type type)
        {
            if (type == null) return false;
            return type.FullName.StartsWith("System.Nullable`1[");
        }

        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <returns></returns>
        public static object GetDefault(this Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// 表示可能的序列类型(集合)
    /// </summary>
    public enum SequenceType
    {
        /// <summary>
        /// 不是一个序列类型
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// 代表 <see cref="System.String"/>.
        /// </summary>
        String,

        /// <summary>
        /// 代表 Array.
        /// </summary>
        Array,

        /// <summary>
        /// 代表 <see cref="System.Collections.BitArray"/>. 非泛型
        /// </summary>
        BitArray,

        /// <summary>
        /// 代表 <see cref="System.Collections.ArrayList"/>. 非泛型
        /// </summary>
        ArrayList,

        /// <summary>
        /// 代表 <see cref="System.Collections.Queue"/>. 非泛型
        /// </summary>
        Queue,

        /// <summary>
        /// 代表 <see cref="System.Collections.Stack"/>. 非泛型
        /// </summary>
        Stack,

        /// <summary>
        /// 代表 <see cref="System.Collections.Hashtable"/>. 非泛型
        /// </summary>
        Hashtable,

        /// <summary>
        /// 代表 <see cref="System.Collections.SortedList"/>. 非泛型
        /// </summary>
        SortedList,

        /// <summary>
        /// 代表 <see cref="Dictionary"/>. 非泛型
        /// </summary>
        Dictionary,

        /// <summary>
        /// 代表 <see cref="ListDictionary"/>. 非泛型
        /// </summary>
        ListDictionary,

        /// <summary>
        /// 代表 <see cref="System.Collections.IList"/>. 非泛型
        /// </summary>
        IList,

        /// <summary>
        /// 代表 <see cref="System.Collections.ICollection"/>. 非泛型
        /// </summary>
        ICollection,

        /// <summary>
        /// 代表 <see cref="System.Collections.IDictionary"/>. 非泛型
        /// </summary>
        IDictionary,

        /// <summary>
        /// 代表n <see cref="System.Collections.IEnumerable"/>. 非泛型
        /// </summary>
        IEnumerable,

        /// <summary>
        /// 代表 自己实现的 <see cref="System.Collections.IEnumerable"/>.
        /// </summary>
        Custom,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.List{T}"/>.
        /// </summary>
        GenericList,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.LinkedList{T}"/>.
        /// </summary>
        GenericLinkedList,

        /// <summary>
        /// 代表 <see cref="Collection{T}"/>.
        /// </summary>
        GenericCollection,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.Queue{T}"/>.
        /// </summary>
        GenericQueue,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.Stack{T}"/>.
        /// </summary>
        GenericStack,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.HashSet{T}"/>.
        /// </summary>
        GenericHashSet,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.SortedList{TKey,TValue}"/>.
        /// </summary>
        GenericSortedList,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.Dictionary{TKey,TValue}"/>.
        /// </summary>
        GenericDictionary,

        /// <summary>
        /// 代表 <see cref="System.Collections.Generic.SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        GenericSortedDictionary,

        /// <summary>
        /// 代表 <see cref="System.Collections.Concurrent.BlockingCollection{T}"/>.
        /// </summary>
        GenericBlockingCollection,

        /// <summary>
        /// 代表 <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>.
        /// </summary>
        GenericConcurrentDictionary,

        /// <summary>
        /// 代表 <see cref="System.Collections.Concurrent.ConcurrentBag{T}"/>.
        /// </summary>
        GenericConcurrentBag,

        /// <summary>
        /// 代表 <see cref="IList{T}"/>.
        /// </summary>
        GenericIList,

        /// <summary>
        /// 代表 <see cref="ICollection{T}"/>.
        /// </summary>
        GenericICollection,

        /// <summary>
        /// 代表 <see cref="IEnumerable{T}"/>.
        /// </summary>
        GenericIEnumerable,

        /// <summary>
        /// 代表 <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        GenericIDictionary,

        /// <summary>
        /// 代表 <see> <cref>ICollection{KeyValuePair{TKey, TValue}}</cref></see>.
        /// </summary>
        GenericICollectionKeyValue,

        /// <summary>
        /// 代表 <see> <cref>IEnumerable{KeyValuePair{TKey, TValue}}</cref></see>.
        /// </summary>
        GenericIEnumerableKeyValue,

        /// <summary>
        /// 代表 自定义实现的 <see cref="IEnumerable{T}"/>.
        /// </summary>
        GenericCustom
    }
}