using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.UserModel
{
	public record LoginDto
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public bool saveMe { get; set; }
	}
}
