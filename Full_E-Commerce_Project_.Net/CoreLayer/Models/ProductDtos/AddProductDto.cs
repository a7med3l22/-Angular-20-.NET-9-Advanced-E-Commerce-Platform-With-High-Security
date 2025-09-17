using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.Dtos
{
	public record AddProductDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal OldPrice { get; set; }
		public decimal NewPrice { get; set; }
		public IFormFileCollection Photos { get; set; }

		public int CategoryId { get; set; } 
	}
}
