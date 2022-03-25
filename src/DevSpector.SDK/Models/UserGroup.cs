namespace DevSpector.SDK.Models
{
    public class UserGroup
    {
        public UserGroup(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; }

        public string Name { get; }
    }
}
