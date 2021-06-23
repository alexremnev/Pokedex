using Newtonsoft.Json;

namespace Pokedex.Services.Models
{
    public class FlavorTextEntry
    {
        [JsonProperty(PropertyName = "flavor_text")]
        public string Description { get; set; }
    }
}