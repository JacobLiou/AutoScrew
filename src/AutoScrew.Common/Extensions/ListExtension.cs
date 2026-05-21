using System.Text.RegularExpressions;

namespace ProtocolSimulationTest.Common.Extensions
{
    public static class ListExtension
    {
        public static string FirstNonEmpty(this IEnumerable<string> strings, string alternateString)
        {
            foreach (string item in strings)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    return item;
                }
            }

            return alternateString;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static List<T> Randomize<T>(this List<T> list)
        {
            var originalList = new List<T>(list);
            var randomList = new List<T>();

            var r = new Random();

            int randomIndex = 0;

            while (originalList.Count > 0)
            {
                randomIndex = r.Next(0, originalList.Count);
                randomList.Add(originalList[randomIndex]);
                originalList.RemoveAt(randomIndex);
            }

            return randomList;
        }

        public static IEnumerable<string> SortNaturally(this IEnumerable<string> strings)
        {
            return strings.OrderBy(x => Regex.Replace(x, @"\d+", match => match.Value.PadLeft(4, '0')));
        }
    }
}