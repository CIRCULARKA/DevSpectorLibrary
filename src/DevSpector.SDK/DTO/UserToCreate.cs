using System.Text.Json.Serialization;

namespace DevSpector.SDK.DTO
{
    public class UserToCreate
    {
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string Login { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string Password { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string GroupID { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string FirstName { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string Surname { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string Patronymic { get; set; }
    }
}
