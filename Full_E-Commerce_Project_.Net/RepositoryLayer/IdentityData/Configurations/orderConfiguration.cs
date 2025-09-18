using CoreLayer.Models.UserModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.appUserConfiguration.Configurations
{
	public class orderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.HasMany(o=>o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade).IsRequired();
			builder.OwnsOne(o => o.DeliveryAddress);
			builder.Property(o => o.PaymentStatus).HasConversion(PS => PS.ToString(), SPS => Enum.Parse<PaymentStatus>(SPS));
			builder.HasOne(o=>o.DeliveryMethod).WithMany().OnDelete(DeleteBehavior.SetNull);
			builder.Property(o => o.SubTotal).HasPrecision(18, 2);
			builder.Property(o => o.DeliveryPrice).HasPrecision(18, 2);

		}
	}
}
