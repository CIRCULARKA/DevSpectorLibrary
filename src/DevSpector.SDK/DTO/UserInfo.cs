namespace DevSpector.SDK.Models
{
    /// <summary>
    /// This is the DTO object needed to provide information to server about user
    /// </summary>
    public class UserInfo
    {
        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string GroupID { get; set; }
    }
}
