using System.Text.Json.Serialization;

namespace DevSpector.SDK.DTO
{
    public class DeviceToCreate
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string InventoryNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string NetworkName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TypeID { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ModelName { get; set; }
    }
}
