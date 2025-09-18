using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models.UserModel
{
	//Gooooooooooooooooooooooooold!!!
	public class Order:BaseClass<int>
	{
		//ToDo Comments
		// Ineed To Make Relation One To Many With AppUser => One User Have Many Order =>Done
		//I Need In Order List Of Order Item That Have Price,OrderItemId,Quentity => I Save Price To Save Price That He Buy This Order With wHICH iF cHANGEC lATER i Will Save The Price at The Moment Of Maked Order=>one Order Will Have Many OrderItems So It Will Be One To Many=>Done
		public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

		//I Need To Make One To One Relation Ship With UsersDeleviredAddresse AND Want Order Own usersDeleviredAddresse Not Be Relationed In New Tables =>Done
		public UsersDeleviredAddresse DeliveryAddress { get; set; } = null!;
		//I Need To Save The Time Of Making Order =>Done
		public DateTime DateTime { get; private set; } = DateTime.UtcNow;//		كده أي كود برا الكلاس مش هيقدر يعدل على DateTime.		//لكن EF Core يقدر يستخدم الـ private setter ويملأ القيمة لما يعمل materialization من الداتا بيز.

		//I Need To Save The Status Of Payment Of Order So I Will Make Enum States=>Done
		public PaymentStatus PaymentStatus { get; set; }=PaymentStatus.Pending;
		//So Now My Order Will Include 
		//Id,AppUserId as Forign Key, usersDeleviredAddresse In The Order Table,Date,Status, List Of Order Item
		//Thus I have Any Information I Need..

		//عاوز اضيف ال صب توتال علشان مضطرش اني اعمل نافيجيشنل بروبيرتي علشان احسبه ف لما اضيف ال اوردر ايتم ف ال اوردر اضيف ال صب توتال معاهم كمان بحيث اني لما اجيبه م الداتا بيز م اضطرش اني اعمل نافيجيشنل بروبيرتي علشان احسب ال صب توتال لاني هبقي حافظه محسوب خلاص 
		public decimal SubTotal { get; set; }

		//هعمل ميثود بقي تحسب ال توتال بحيث اني لما اجيب ال اوردر م الداتا بيز واعوز احسب ال توتال استدعي الميثود ع طول تسهل عليا بدل م اجمع ب ايدي بره 
		// صب توتال + ديليفيري برايس
		public decimal TotalPrice()
		{
			//انا شايف اني احفظ سعر ال دليفري ميثود ف ال داتا بيز افضل بحيث اني لما احتاج احسب ال توتال مضطرش اني اعمل انكلود لل دليفري ميثود بس كده فيه حاجة اني لما اضيف اوردر اصلا مش هضيف الا دليفري ميثود اي دي لاني هستخدم دليفري ميثود من اللي موجودين ب الفعل , ف بكده لازم وانا بضيف الاوردر ابحث الاول عن سعر ال دليفري ميثود اللي  ب ال اي دي ده وبعدين اضيف السعر واضيف ال اي دي عادي    ..
			//والميزة اللي انا شايفها حلوة اني اعمل كده ان لو اتغير السعر بتاع ال دليفري ميثود بعدين هبقي محتفظ ب السعر وقت عملية الاوردر بحيث اني مجيش اتفاجي بعدين ان السعر رفع فجأه بعد م عملت الاوردر بسعر معين 

			return SubTotal + DeliveryPrice;
		}
		public decimal DeliveryPrice { get; set; }


		public DeliveryMethod? DeliveryMethod { get; set; } // كل اوردر ليه طريقه توصيل واحده انما طريقة التوصيل الواحده ممكن اجيب منها ليسته من ال اوردرات اللي اتوصلت من خلالها يبقي علاقه وان دليفري وز ميني اوردر 
														   //Gold	//  لو ضفت الدليفري ميثود مع الاوردر لما اضيف الاوردر كده بضيف دليفري ميثود جديد انما لما اضيف دليفري ميثود اي دي مع الاوردر من غير دليفري ميثود  ف بكده مضافش دليفري ميثود جديده وبيستخدم من اللي موجود وده اللي مطلوب 
		public int? DeliveryMethodId { get; set; }


		// مش لازم اضيف ال ايميل بتاع ال يوزر لاني ب الفعل رابطه ب ال يوزر اي دي
	}
}
