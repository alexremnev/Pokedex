using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pokedex.Services.Models
{
    public class PokemonDetails
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "flavor_text_entries")]
        public IList<FlavorTextEntry> FlavorTextEntries { get; set; }

        [JsonProperty(PropertyName = "habitat")]
        public Habitat Habitat { get; set; }

        [JsonProperty(PropertyName = "is_legendary")]
        public bool IsLegendary { get; set; }
    }
}
