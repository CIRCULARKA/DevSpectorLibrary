namespace DevSpector.SDK.Models
{
    public class DeviceType
    {
        public DeviceType(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; }

        public string Name { get; }
    }
}
