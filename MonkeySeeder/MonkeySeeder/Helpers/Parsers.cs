using System.Linq;
using System.Net;

namespace MonkeySeeder.Helpers
{
    public static class Parsers
    {
        public static bool TryParseIPEndpoint(string s, out IPEndPoint endPoint)
        {
            endPoint = null;
            if (string.IsNullOrEmpty(s))
                return false;
            if (!s.Contains(":"))
                return false;
            var splitIP = s.Split(':');
            if (splitIP?.Count() != 2)
                return false;
            if (!IPAddress.TryParse(splitIP[0], out var ip))
                return false;
            if (!int.TryParse(splitIP[1], out var port))
                return false;
            endPoint = new IPEndPoint(ip, port);
            return endPoint != null;
        }
    }
}