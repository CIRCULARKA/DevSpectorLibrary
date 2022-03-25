namespace DevSpector.SDK.Models
{
    public class ApplianceType
    {
        public ApplianceType(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; }

        public string Name { get; }
    }
}
