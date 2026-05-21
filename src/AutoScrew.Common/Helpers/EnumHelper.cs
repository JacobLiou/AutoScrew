using System.ComponentModel;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T value) where T : Enum
        {
            var type = typeof(T);
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>();

            return attributes.FirstOrDefault()?.Description ?? value.ToString();
        }

        public static T[] GetValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static List<string> GetNames<T>() where T : Enum
        {
            var names = new List<string>();
            var values = GetValues<T>();
            foreach (var value in values)
            {
                string name = Enum.GetName(typeof(T), value)!;
                names.Add(name);
            }

            return names;
        }

        public static List<int> GetIntValue<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => Convert.ToInt32(v)).ToList();
        }
    }
}