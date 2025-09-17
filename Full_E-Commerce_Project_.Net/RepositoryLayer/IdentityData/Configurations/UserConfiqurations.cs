using CoreLayer.Models.UserModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IdentityData.Configurations
{
	public class UserConfiqurations : IEntityTypeConfiguration<AppUser>
	{
		public void Configure(EntityTypeBuilder<AppUser> builder)
		{
			builder.HasOne(u => u.usersMainAddresse).WithOne().HasForeignKey<UsersMainAddresse>().OnDelete(DeleteBehavior.Cascade).IsRequired();// عاوز ال فورين كي يبقي عند ال يوزر ادريس ف كده تمام هيبقي موجود ف اليوزر ادريس AppUserId  // علشان لو مسحت ال يوزر ال ادريس يمسح معاه // علشان مينفعش اضيف ال ادريس من غير م يكون مرتبط ب يوزر يعني ال فورين كي ال ف ال ادريس مينفعش يبقي ب نال


			builder.HasOne(u => u.usersDeleviredAddresse).WithOne().HasForeignKey<UsersDeleviredAddresse>().OnDelete(DeleteBehavior.Cascade).IsRequired();// عاوز ال فورين كي يبقي عند ال دليفري ادريس ف كده تمام هيبقي موجود ف اليوزر ادريس AppUserId// علشان لو مسحت ال يوزر ال ادريس يمسح معاه



		}
	}
}
