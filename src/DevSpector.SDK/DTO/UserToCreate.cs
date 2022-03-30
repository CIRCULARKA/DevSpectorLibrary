namespace DevSpector.SDK.DTO
{
    public class UserToCreate
    {
		public string Login { get; set; }

		public string Password { get; set; }

        public string GroupID { get; set; }

		public string FirstName { get; }

		public string Surname { get; }

		public string Patronymic { get; }
    }
}
