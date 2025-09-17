using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
	public class Photo:BaseClass<int>
	{
		// مش عاوز حاليا من ال فوتو اروح ل اي نافيجيشنل بروبيرتي ف مش هكتبهم 
		public string ImageName { get; set; }
		public int ProductId { get; set; }
		public string Hash { get; set; } // علشان اعرف الصور متكررة قبل كده ولا لا من الهاش مش من النيم !

	}
}
