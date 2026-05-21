using Humanizer;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class ReadableStringHelper
    {
        public static string GetDateTimesDuration(DateTimeOffset? date1, DateTimeOffset? date2)
        {
            if (date1 == null || date2 == null)
                return "-";

            var span = (TimeSpan)(date2 - date1);

            return span.Humanize();
        }
    }
}