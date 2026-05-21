using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProtocolSimulationTest.Common.Extensions
{
    public static class GeneralExtension
    {
        public static IList<string> GetAllPropertyValues<T>(this T t)
        {
            var valueList = new List<string>();
            var properties = t.GetType().GetProperties();
            foreach (var property in properties)
            {
                valueList.Add(property.GetValue(t)?.ToString()!);
            }

            return valueList;
        }

        public static string ClassToString<T>(this T data) where T : class
        {
            var propertys = data.GetType().GetProperties();
            var str = string.Empty;
            foreach (var item in propertys)
            {
                str += $"{item.Name}:{item.GetValue(data)},";
            }
            return str.TrimEnd(',');
        }

        public static byte[] ObjectToByteArray(this object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(this byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static T ByteArrayToObject<T>(this byte[] data)
        {
            if (data == null) return default!;
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        /// <summary>
        /// 获取以0点0分0秒开始的日期
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime GetStartDateTime(DateTime d)
        {
            if (d.Hour != 0)
            {
                var year = d.Year;
                var month = d.Month;
                var day = d.Day;
                var hour = "0";
                var minute = "0";
                var second = "0";
                d = Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
            }
            return d;
        }

        /// <summary>
        /// 获取以23点59分59秒结束的日期
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime GetEndDateTime(DateTime d)
        {
            if (d.Hour != 23)
            {
                var year = d.Year;
                var month = d.Month;
                var day = d.Day;
                var hour = "23";
                var minute = "59";
                var second = "59";
                d = Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
            }
            return d;
        }

        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <typeparam name="T">要验证的对象的类型</typeparam>
        /// <param name="data">要验证的对象</param>
        public static bool IsNullOrEmpty<T>(T data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }

        //// 用于跟踪已克隆对象的字典，防止循环引用导致的无限递归
        //// 使用 ReferenceEqualityComparer 确保基于引用地址比较对象，而不是 Equals() 方法
        //private static readonly Dictionary<object, object> _visited =
        //    new Dictionary<object, object>(ReferenceEqualityComparer.Instance);

        ///// <summary>
        ///// 使用反射执行对象的深拷贝。
        ///// </summary>
        ///// <typeparam name="T">要克隆的对象类型。</typeparam>
        ///// <param name="original">要克隆的原始对象。</param>
        ///// <returns>原始对象的深拷贝副本。</returns>
        //public static T DeepClone<T>(this T original)
        //{
        //    // 清空上次克隆的记录（如果希望每次调用都是独立的）
        //    // 注意：如果在同一操作中克隆多个相互关联的对象，则不应在此处清除，
        //    // 而应在整个操作开始前清除，或将 _visited 作为参数传递。
        //    // 为了简单起见，这里每次调用都清除。如果性能敏感或需要跨多个调用跟踪，请调整。
        //    _visited.Clear();
        //    return (T)CloneObjectInternal(original);
        //}

        //private static object CloneObjectInternal(object original)
        //{
        //    // 1. 处理 null
        //    if (original == null)
        //    {
        //        return null;
        //    }

        //    Type type = original.GetType();

        //    // 2. 处理值类型和字符串 (字符串是引用类型但不可变，通常视为值类型处理)
        //    if (type.IsValueType || original is string)
        //    {
        //        return original; // 值类型直接返回，字符串共享引用即可
        //    }

        //    // 3. 处理循环引用：如果对象已克隆过，直接返回克隆后的实例
        //    if (_visited.TryGetValue(original, out object alreadyCloned))
        //    {
        //        return alreadyCloned;
        //    }

        //    // 4. 处理数组
        //    if (type.IsArray)
        //    {
        //        Type elementType = type.GetElementType();
        //        Array originalArray = (Array)original;
        //        Array clonedArray = Array.CreateInstance(elementType, originalArray.Length); // TODO: 处理多维数组

        //        // 将新创建的克隆数组（此时为空）添加到 visited 字典中，用于处理循环引用
        //        _visited.Add(original, clonedArray);

        //        for (int i = 0; i < originalArray.Length; i++)
        //        {
        //            // 递归克隆数组元素
        //            clonedArray.SetValue(CloneObjectInternal(originalArray.GetValue(i)), i);
        //        }
        //        return clonedArray;
        //    }

        //    // 5. 处理引用类型（类）
        //    // 使用 FormatterServices.GetUninitializedObject 创建对象实例，
        //    // 这样可以绕过构造函数，避免构造函数中的副作用或依赖。
        //    // 注意：这需要 System.Runtime.Serialization.Formatters 支持。
        //    // 在 .NET Core / .NET 5+ 中，可能需要添加 NuGet 包 System.Runtime.Serialization.Formatters。
        //    object clonedObject = FormatterServices.GetUninitializedObject(type);

        //    // 将新创建的克隆对象（此时为空）添加到 visited 字典中，用于处理循环引用
        //    _visited.Add(original, clonedObject);

        //    // 获取所有实例字段（包括公有、非公有、继承的）
        //    // BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        //    // 如果需要拷贝基类的字段，需要循环 type.BaseType
        //    List<FieldInfo> fields = new List<FieldInfo>();
        //    Type currentType = type;
        //    while (currentType != null && currentType != typeof(object))
        //    {
        //        fields.AddRange(currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
        //        currentType = currentType.BaseType;
        //    }

        //    foreach (FieldInfo field in fields)
        //    {
        //        // 跳过特殊类型，如指针 (IntPtr, UIntPtr) 或委托（根据需要决定是否克隆）
        //        if (field.FieldType.IsPointer || typeof(Delegate).IsAssignableFrom(field.FieldType))
        //        {
        //            // 可以选择跳过，或者根据需要实现特定的克隆逻辑
        //            continue;
        //        }

        //        object originalFieldValue = field.GetValue(original);
        //        // 递归克隆字段的值
        //        object clonedFieldValue = CloneObjectInternal(originalFieldValue);
        //        // 将克隆后的值设置到新对象的字段上
        //        field.SetValue(clonedObject, clonedFieldValue);
        //    }

        //    return clonedObject;
        //}

        //// 用于比较对象引用地址的辅助类
        //private class ReferenceEqualityComparer : IEqualityComparer<object>
        //{
        //    public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();
        //    private ReferenceEqualityComparer() { }

        //    public new bool Equals(object x, object y)
        //    {
        //        return ReferenceEquals(x, y); // 关键：使用引用比较
        //    }

        //    public int GetHashCode(object obj)
        //    {
        //        // 使用对象的原始哈希码（可能由 System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode 实现）
        //        return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        //    }
        //}

        /// <summary>
        /// 泛型对象深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T source) where T : class, new()
        {
            if (source == null)
                return null;

            return (T)DeepCloneObject(source, new Dictionary<object, object>());
        }

        private static object DeepCloneObject(object source, IDictionary<object, object> visited)
        {
            if (source == null)
                return null;

            var typeToClone = source.GetType();

            // Prevent circular references
            if (visited.ContainsKey(source))
                return visited[source];

            // Value types or strings are returned directly
            if (typeToClone.IsValueType || typeToClone == typeof(string))
                return source;

            // Arrays
            if (typeToClone.IsArray)
            {
                var elementType = typeToClone.GetElementType();
                var array = (Array)source;
                var cloned = Array.CreateInstance(elementType, array.Length);
                visited[source] = cloned;

                for (int i = 0; i < array.Length; i++)
                {
                    var clonedElem = DeepCloneObject(array.GetValue(i), visited);
                    cloned.SetValue(clonedElem, i);
                }
                return cloned;
            }

            // IList implementations (e.g. List<T>)
            if (typeof(IList).IsAssignableFrom(typeToClone))
            {
                var listType = typeToClone;
                var clonedList = (IList)Activator.CreateInstance(listType);
                visited[source] = clonedList;

                foreach (var item in (IList)source)
                {
                    clonedList.Add(DeepCloneObject(item, visited));
                }
                return clonedList;
            }

            // POCO objects
            var clone = Activator.CreateInstance(typeToClone);
            visited[source] = clone;

            foreach (var prop in typeToClone.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .Where(p => p.CanRead && p.CanWrite))
            {
                var propValue = prop.GetValue(source);
                var clonedValue = DeepCloneObject(propValue, visited);
                prop.SetValue(clone, clonedValue);
            }

            return clone;
        }
    }
}