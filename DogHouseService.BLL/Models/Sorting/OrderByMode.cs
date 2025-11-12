using System.Text.Json.Serialization;

namespace DogHouseService.BLL.Models.Sorting
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderByMode
    {
        Asc,
        Desc
    }
}
