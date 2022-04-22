namespace DevSpector.SDK.Models
{
	/// <summary>
	/// Client-side user model
	/// </summary>
	public class User
	{
		public User(
			string id,
			string accessToken,
			string login,
			string group,
			string firstName,
			string surname,
			string patronymic
		)
		{
			ID = id;
			AccessToken = accessToken;
			Login = login;
			Group = group;
			FirstName = firstName;
			Surname	 = surname;
			Patronymic = patronymic;
		}

		public string ID { get; }

		public string AccessToken { get; }

		public string Login { get; }

		public string Group { get; }

		public string FirstName { get; }

		public string Surname { get; }

		public string Patronymic { get; }
	}
}
