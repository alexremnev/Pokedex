using Newtonsoft.Json;

namespace Pokedex.Services.Models
{
    public class Habitat
    {

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}