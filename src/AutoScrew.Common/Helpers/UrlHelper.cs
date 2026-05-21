using System.Text.RegularExpressions;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class UrlHelper
    {
        public static bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        public static bool IsIPv4Address(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            return Regex.IsMatch(ip,
                "^((2((5[0-5])|([0-4]\\d)))|([0-1]?\\d{1,2}))(\\.((2((5[0-5])|([0-4]\\d)))|([0-1]?\\d{1,2}))){3}$");
        }
    }
}