using Newtonsoft.Json;

namespace Rihal_Cinema.Dtos
{
    public class RihalApiResults
    {
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("main_cast")]
        public List<string> MainCasts { get; set; }

        public string Director { get; set; }
        public int Budget { get; set; }
    }
}
