using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pokedex.Services
{
    public class Utils
    {
        public static async Task<T> ReadResultAsync<T>(HttpResponseMessage responseMessage)
        {
            var stringResult = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }

        public static StringContent ConvertToStringContent(object value)
        {
            var content = JsonConvert.SerializeObject(value);
            return new StringContent(content, Encoding.Unicode, "application/json");
        }
    }
}
