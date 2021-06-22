using Newtonsoft.Json;

namespace Pokedex.Services.Models
{
    public class FormsDescription
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}