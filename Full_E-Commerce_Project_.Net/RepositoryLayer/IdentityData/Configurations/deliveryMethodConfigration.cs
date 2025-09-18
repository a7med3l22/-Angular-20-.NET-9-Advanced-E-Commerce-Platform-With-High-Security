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
	public class deliveryMethodConfigration : IEntityTypeConfiguration<DeliveryMethod>
	{
		public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
		{
			builder.Property(DM=>DM.Price).HasPrecision(4,2);

		}
	}
}
