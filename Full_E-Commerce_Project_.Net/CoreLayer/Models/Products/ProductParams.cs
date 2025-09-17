using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.Products
{
	public record ProductParams
	{
		public string? SortBy { get; set; }
		public int? CategoryId { get; set; }
		public string? Search { get; set; }

		//Pagination // PageNumber,PageSize

		private int _pageNumber=1;
		//[Gold]
		// اللي بيعمله ال بايندج انه لو الاسترنج مش نلابول هيرجع ايرور 
		// لو مبعتش قيمة ف ال بايندج هيعمل نيو من ال ريكورد ده تمام وهيرجع القيمة اللي موجوده ف ال بروبيرتي  سواء القيمة دي كانت ال ديفولت من ال CLR او انشيلايز انا اللي حاطتها من غير م يعمل سيت لاي حاجة 
		// اكنه كده ب الظبط  new ProductParams();
		// ولكن لو حطيت قيم بيروح لل سيتير بتاع ال حاجات اللي اتحطلها قيمة 
		// اكني عملت كده ب الظبط new ProductParams(){PageNumber=5};
		// بس زي م قولت ان لو مبعتش قيمة ل استرنج والاسترنج ده مش نلابول هيطلع ايرور وهيقولي انه ريكويرد 
		public int PageNumber 
		{
			get
			{
				return _pageNumber;
			}
			set
			{
				_pageNumber = (value < 1) ? 1 : value;
			}
		}

		private int _pageSize=6 ; 
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = (value>1&&value<11)? value:6;
			}
		}
	}
}
