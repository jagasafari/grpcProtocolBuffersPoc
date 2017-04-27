using Newtonsoft.Json;
using System.Text;

namespace Messages
{
    public class Serializer<T>
    {
        public static byte[] ToBytes(T obj)
        {
            return new UTF8Encoding().GetBytes(
                JsonConvert.SerializeObject(obj));
        }

        public static T FromBytes(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(
                new UTF8Encoding().GetString(bytes));
        }
    }
}
