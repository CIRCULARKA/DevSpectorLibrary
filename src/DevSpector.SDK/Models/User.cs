namespace DevSpector.SDK.Models
{
	/// <summary>
	/// Client-side user model
	/// </summary>
	public class User
	{
		public User(string accessToken, string login, string group)
		{
			AccessToken = accessToken;
			Login = login;
			Group = group;
		}

		public string AccessToken { get; }

		public string Login { get; }

		public string Group { get; }

		public string FirstName { get; }

		public string Surname { get; }

		public string Patronymic { get; }
	}
}
