using Microsoft.AspNetCore.WebUtilities;

namespace Full_E_Commerce_Project.Handle_MiddleWares
{
	public class MyException:Exception
	{
		public readonly int code;
		public readonly string message;
		public readonly string? develoberMessage;
		public readonly List<string>? develoberMessages;

		// عاوز ال كود وال ماسدج 
		// علشان لو عوزت ابعت رسالة لل ديفيلوبر كده عملنا اوفر رايد ع ال ديفولت ماسدج او ممكن مبعتش حاجة للاب وهو هيكون ف ال ماسدج بتاع الاب القيمة الافتراضية اللي هي حاجة شبه دي "Exception of type 'Full_E_Commerce_Project.Handle_MiddleWares.MyException' was thrown."
		public MyException(int code,string?message=null,string?develoberMessage=null,List<string>? develoberMessages=null) :base(develoberMessage)
		{
			if(message == null)
			{
				message=ReasonPhrases.GetReasonPhrase(code)??"UnKnown Error!!";
			}

			this.code = code;
			this.message = message;
			this.develoberMessage = develoberMessage;
			this.develoberMessages = develoberMessages;
		}
	}
}
