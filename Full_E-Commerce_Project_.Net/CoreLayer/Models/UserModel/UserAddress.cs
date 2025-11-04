namespace CoreLayer.Models.UserModel
{
	public record UsersMainAddresse
	{
		public int Id { get; set; }
		// هاخد ال فريست نيم وال لست نيم من الاب يوز
		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public string Country { get; set; }
		public string AppUserId { get; set; }// Foreign Key


	}

	public record SetUsersMainAddresse
	{

		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public string Country { get; set; }

	}
}