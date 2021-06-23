using Newtonsoft.Json;

namespace Pokedex.Services.Models
{
    public class NameEntity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}