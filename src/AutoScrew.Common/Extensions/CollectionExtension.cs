using System.Collections.ObjectModel;

namespace ProtocolSimulationTest.Common.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// 集合是否为空
        /// </summary>
        /// <param name="collection"> 要处理的集合 </param>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <returns> 为空返回True，不为空返回False </returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// 给IEnumerable拓展ForEach方法
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="func">委托方法</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> func)
        {
            foreach (T item in collection)
            {
                func(item);
            }
        }

        /// <summary>
        /// 将集合元素转换成字符串，再以指定的分隔符衔接，拼成一个字符串返回。默认分隔符为空格
        /// </summary>
        /// <param name="collection"> 要处理的集合 </param>
        /// <param name="separator"> 分隔符，默认为空格 </param>
        /// <returns> 拼接后的字符串 </returns>
        public static string ToSpliceString<T>(this IEnumerable<T> collection, string separator = " ")
        {
            return string.Join(separator, collection);
        }

        public static bool StartWith<T>(this IEnumerable<T> collection, T value)
        {
            foreach (T item in collection)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EndWith<T>(this IEnumerable<T> collection, T value)
        {
            foreach (T item in collection)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NotEmpty<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }

        public static T FindFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (!(source is T[] source2))
            {
                return source.FirstOrDefault(predicate)!;
            }

            return source2.FindFirst(predicate);
        }

        public static Collection<T> AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var each in items)
            {
                collection.Add(each);
            }

            return collection;
        }
    }
}