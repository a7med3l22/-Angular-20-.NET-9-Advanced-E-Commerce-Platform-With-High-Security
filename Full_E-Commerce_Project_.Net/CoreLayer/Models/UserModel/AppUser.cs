using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.UserModel
{
	public class AppUser:IdentityUser
	{
		//public int Id { get; set; } // كده كده وارث اي دي 
		public string FirstName { get; set; }
		public DateTime CreationDate { get; set; }=DateTime.UtcNow;
		public string LastName { get; set; }
		//public string? PassToken { get; set; }
		// وان تو وان ريليشن بس عاوز ال ابب يوزر يبقي هو ال متبوع وال ادريس هو التابع 
		public UsersMainAddresse usersMainAddresse { get; set; } // علشان ده يبقي الاساسي وهو بيتسجل وبعد كده لما يدخل ال ادريس بتاع الدليفري يبقي يعرضله ده فيه لو عمل سيف يتحفظ ف ال ديليفري ادريس 
		
		
		public ICollection<Order> orders { get; set; }=new HashSet<Order>();
	}
}
