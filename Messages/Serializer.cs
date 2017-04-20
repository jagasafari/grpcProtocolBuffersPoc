using Newtonsoft.Json;
using System.Text;

namespace Messages
{
    public class Serializer<T>
    {
        public static byte[] ToBytes(T obj)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
        }
        public static T FromBytes(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(bytes));
        }
    }
}
