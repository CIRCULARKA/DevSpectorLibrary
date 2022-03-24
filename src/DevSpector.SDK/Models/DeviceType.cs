namespace DevSpector.SDK.Models
{
    public class DeviceType
    {
        public DeviceType(string typeID, string typeName)
        {
            ID = typeID;
            Name = typeName;
        }

        public string ID { get; }

        public string Name { get; }
    }
}
