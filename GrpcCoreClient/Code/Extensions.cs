using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace GrpcCoreClient.Code
{
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
        {
            return new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
        }

        public static string AsJsonString(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}