using CoreLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Data.Migrations
{
	public class ProductConfigurations : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{

			builder.Property(x => x.NewPrice).HasPrecision(18, 2);
			builder.Property(x => x.OldPrice).HasPrecision(18, 2);
			builder.HasOne(navigationExpression: x => x.Category).WithMany(c => c.AllProductsInCategory).OnDelete(DeleteBehavior.Cascade).IsRequired();// هو عرف عنده فورين كي تلقائي اسمه CategoryId , ف لو حبيت ابعت فاليو لل كولوم ده ف الداتا بيز اضيف بروبيرتي نفس الاسم ع طول وهو هيضيف فيه ف ال داتا بيز من غير م اعمل اي حاجة And ولو انا كنت ضايفه عندي قبل م اعمل مايجريشن ف هو هيعرف انه ال فورين كي تلقائي من غير م احدده كده لان انا مسميه ب اسم هو متبرجع انه يخليه ال فورين كي لو لقي بروبيرتي نفس الاسم اللي متوقعه ف مش محتاج اعمل كده .HasForeignKey(p=>p.CategoryId)  
			//builder.HasOne(navigationExpression: x => x.Category).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired();// هو عرف عنده فورين كي تلقائي اسمه CategoryId , ف لو حبيت ابعت فاليو لل كولوم ده ف الداتا بيز اضيف بروبيرتي نفس الاسم ع طول وهو هيضيف فيه ف ال داتا بيز من غير م اعمل اي حاجة And ولو انا كنت ضايفه عندي قبل م اعمل مايجريشن ف هو هيعرف انه ال فورين كي تلقائي من غير م احدده كده لان انا مسميه ب اسم هو متبرجع انه يخليه ال فورين كي لو لقي بروبيرتي نفس الاسم اللي متوقعه ف مش محتاج اعمل كده .HasForeignKey(p=>p.CategoryId)  
			builder.HasMany(p=>p.Photos).WithOne().OnDelete(DeleteBehavior.Cascade).IsRequired();
		}
	}
}

