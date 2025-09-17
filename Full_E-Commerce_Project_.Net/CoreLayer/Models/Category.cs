using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
	public class Category:BaseClass<int>
	{
		public string Name { get; set; }
		public string Description { get; set; }
		// مش عاوز حاليا من ال كاتيجوري اروح ل اي نافيجيشنل بروبيرتي ف مش هكتبهم 
		public ICollection<Product> AllProductsInCategory { get; set; } = new HashSet<Product>();

	

	}
}
