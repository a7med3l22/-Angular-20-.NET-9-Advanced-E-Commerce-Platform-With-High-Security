using CoreLayer.Handle_Exception;
using CoreLayer.Models.UserModel;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Full_E_Commerce_Project.Controllers
{

	public class AccountController : GrandController
	{
		private readonly UserManager<AppUser> userManager;
		private readonly IConfiguration configuration;
		private readonly AccountRepo accountRepo;

		public AccountController(UserManager<AppUser> userManager, IConfiguration configuration, AccountRepo accountRepo)
		{
			this.userManager = userManager;
			this.configuration = configuration;
			this.accountRepo = accountRepo;
		}
		//login - register(Done) - logout - edit profile - change password - forget password - reset password 

		//register

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			// logic
			//ModelState(ModelStateDictionary)=>Values(IEnumerable<ModelStateEntry>)=>Errors(ModelErrorCollection : Collection<ModelError>) 
			if (!ModelState.IsValid)
			{
				var errorMessages = ModelState.Values.SelectMany(v => v.Errors)// لان ال اي اينابرابول موديل استيت فيه ف كل موديل استيت اي اينامرابول موديل ايرور // كده انا عملت سيليكت ل اي اينامرابول موديل ايرور 
				.Select(e => e.ErrorMessage)// كده بقيت عبارة عن اي اينامرابول سترينج
				.ToList();// كده بقيت ليست سترينج

				return BadRequest(new ControllerHandleError(code: 400, messages: errorMessages));
			}
			;

			AppUser? UserWithThisEmail = await userManager.FindByEmailAsync(registerDto.Email);
			if (UserWithThisEmail is not null)
			{
				return BadRequest(new ControllerHandleError(400, "Email is already in use"));
			}
			;
			var UserWithThisusername = await userManager.FindByNameAsync(registerDto.UserName);
			if (UserWithThisusername is not null)
			{
				return BadRequest(error: new ControllerHandleError(400, "User Name is already in use"));

			}
			;
			AppUser user = new AppUser()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				Email = registerDto.Email,
				UserName = registerDto.UserName,
				usersMainAddresse = new()
				{
					City = registerDto.UserAddress.City,
					Country = registerDto.UserAddress.Country,
					Street = registerDto.UserAddress.Street,
					State = registerDto.UserAddress.State,
					ZipCode = registerDto.UserAddress.ZipCode
				}
			};

			// to be continue




			var result = await userManager.CreateAsync(user, registerDto.Password);//بعد الخطوة دي بيبتدي يضيف ال اي دي بتاع ال يوزر لل يوزر 
			if (!result.Succeeded)
			{
				var errorMessages = result.Errors.Select(e => e.Description).ToList();
				return BadRequest(new ControllerHandleError(code: 400, messages: errorMessages));
			}
			;
			var TokenEmail = await userManager.GenerateEmailConfirmationTokenAsync(user);

			try
			{

				accountRepo.sendEmail(user.Email, Subject: "ConFirm Email", "Account/SendEmail", user, TokenEmail);
			}
			catch
			{
				await userManager.DeleteAsync(user);
				throw new MyException(400, "Cant Send Email Successfully!!");
			}
			// عاوز اعمل تراي كاتش ولو حصل ايورو يرجع الحاجة لل داتا بيز

			return Ok(new
			{
				message = "Registered Successfully,We Sent An Confirmation Email To You,Please Check It..",
				user.UserName,
				user.Email
		
			});

			//To Be Continue [Email Verification,Reset-Password]

			//ToDo	Comments
			//1-CREATE Send Email Calss [To Send Eamil To Gmail]
			//In This Class We Will Use Method SendEmail That Have Parameters (To,Subject,Component, user,Token-Email)
			//And In This Method It Will Send Email With Link That Redirect To Component "Account/SendEmail" With Email And Token-Email
			//(Done)

			//And If Component Is verifyEmail
			//We Will Click On Button Confirm Email In Gmail
			//Which Click On Button We Re-Direct To Frontend Url Component With Send Email-User And Token-Email In This Url
			//In Angular Component onInit We Will Take UserEmail And Token-Email From Url
			//(Done)
			//And Call Action VerifyEmail In Account Controller With Send Email And Token-Email
			//In VerifyEmail Action We Will Check If Email And Token-Email Are Valid Or Not With UserManager ConfirmEmailAsync Method
			//If Valid It Will Change The EmailConfirmed Automatically To True
			//If False It Will Return Bad Request With Message Invalid Link
			//(Done)


			//And If Component Is ResetPassword
			//We Will Click On Button Reset Password In Gmail
			//Which Click On Button We Re-Direct To Frontend Url Component With Send Email-User And Token-ResetPassword In This Url
			//In Angular Component OnInit We Will Take UserEmail And Token-ResetPassword From Url
			//And Call Action IsValidPassLink In Account Controller With Send Email And Token-ResetPassword =>(Done)
			//If Not Valid It Will Return Bad Request With Message Invalid Link =>(Done)
			//And If Valid It Will Return Ok With UserEmail And Token-ResetPassword In Returned Value =>(Done)
			//And Here We Will ReDirect To ResetPassword Component With UserEmail And Token-ResetPassword In Url
			//And If We Type Ok On password And ConfirmPassword And Click On Button It Will Call Action ResetPassword In Account Controller
			//And In This Action We Will Receive UserEmail And Token-ResetPassword And New Password And ConfirmPassword
			//And If Valid It Will Change The Password And Return Ok
			//If Email Or Token-ResetPassword Not Valid It Will Return Bad Request With Message Invalid Link
			//If Password And ConfirmPassword Not Match It Will Return Bad Request With Message Password And Confirm Password Do Not Match
			//(Done All)






			//2-In Login Action
			//We Will Check If EmailConfirmed Is True Or False
			//If False We Will Resend The Email Again Via SendEmail Method That Have In It Prameter  New New Token-Email We Get It From UserManager GenerateEmailConfirmationTokenAsync Method And Email , Finally We Retrun Bad Request With Message To Check Your Email
			//If True It Will Create An Jwt Token And Save It In Cookie


			//3-Handle Cookies In The Application
			//It Will Request To Send The Cookie With Each Request


			// ممكن تغير ال لايف تايم بتاع ال توكين اللي بيعملوا ال ايدينتتي بتاع ال ريسيت باس وال كونفيرم ايميل وخلي اللايف تايم بتاع ال كوكيز وال جي دبلو تي ب سبع ايام 

			/*
					
								انا هزود ف كلاس ال اب يوزر اللي انا عامله  نلابول بروبيرتي،
								PassToken
					
								وبعدين لما اطلب منه يغير الباسوورد وهو بينشأ التوكين يحفظهولي ف ال داتا بيز  
								قبل م يغير الباسوورد يتأكد من التوكين ف الداتا بيز  
			  وبعد م يغيرهم بنجاح يخليه ب نال وبكده اللينك الواحد مش هعرف استخدمه اكتر من مرة ومش هعرف استخدم لنك قديم كمان بفضل ال اتشك اللي بيعملها ع التوكين لانها هتكون ب نال بعد اول نجاح ف التغيير  
			
			واحفظها انكود ف الداتا بيز 

			///// ع فكرة التوكين بتاع ال ريسيت باس بيبقي ان فاليد تلقائي اصلا اول م اي حاجة تتغير ف اليوزر ف اول لما الباس يتغير اي توكين قديم اتنشأ قبل اخر تغيير بيكون بلا قيمة
			///ف بكده اللي عملته اني حفظت التوكين ف الداتا بيز ملهوش اي لازمة غير اني استخدم اخر توكين اتنشأ بس قبل اي تغيير ف اليوزر وطبعا لازم يكون التوكين ذات نفسه صالح كمان من حيث الوقت بتاعه وكده انه ميكونش عدي الوقت اللي محدداه ال اي دينتتي ال هو 24 ساعة
				
			// شيل ال انتا عملته بتاع انك حفظت التوكين ف الداتا بيز علشان ملهوش اهمية 
			//(Done All)

			//

			
			

			 /////
			 ف ال EmailConfirmed مش محتاج اخزن التوكين والكلام ده لا انا هتشك علي ال EmailConfirmed لو ترو يعمله رسالة انه بالفعل متأكتف ويخرج ولو فولس طبعا يتأكد م التوكين //Done
			
				//(Done All)
			 
			 */

		}
		[HttpGet("VerifyEmail")]
		public async Task<IActionResult> VerifyEmail(string email, string Token)
		{

			await accountRepo.VerifyEmailAsync(email, Token);
			return Ok(new { message = "Confirmed Successfully" });


		}
		[HttpGet("IsValidPassLink")]
		public async Task<IActionResult> IsValidPassLink(string email, string Token)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return BadRequest(new ControllerHandleError(400, "email Is Not Reqistered!!"));
			}
			var decodedToken = WebUtility.UrlDecode(Token).Replace(" ", "+"); // علشان يعرف انه جاي من ال يو ار ال ف يهندله ع الاساس ده 

			//if (user.PassToken != decodedToken)
			//{
			//	return BadRequest(new ControllerHandleError(400, "invalid Link!!"));
			//}

			var isTokenValid = await userManager.VerifyUserTokenAsync(
					user,
					TokenOptions.DefaultProvider, // دايمًا Default
					"ResetPassword",
					decodedToken
					);

			if (isTokenValid)
			{
				return Ok(new { message = "Valid Link" });
			}



			return BadRequest(new ControllerHandleError(400, "Not Valid Link"));


		}

		[HttpGet("resetPassword")]
		public async Task<IActionResult> ResetPassword(string email, string Token, string newPassword)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return BadRequest(new ControllerHandleError(400, "Not Found This Email!!"));
			}
			var decodedToken = WebUtility.UrlDecode(Token).Replace(" ", "+"); // علشان يعرف انه جاي من ال يو ار ال ف يهندله ع الاساس ده 

			//if (user.PassToken != decodedToken)
			//{
			//	return BadRequest(new ControllerHandleError(400, "Cant Change Password Twice!!!!"));
			//}

			var resetPassword = await userManager.ResetPasswordAsync(user, decodedToken, newPassword);

			if (resetPassword.Succeeded)
			{
				//user.PassToken = null;
				//await userManager.UpdateAsync(user);
				return Ok(new { message = "Reseted Successfully" });
			}
			List<string> Errors = resetPassword.Errors.Select(e => e.Description).ToList();


			return BadRequest(new ControllerHandleError(400, messages: Errors));


		}

		[HttpGet("forgetPassword")]
		public async Task<IActionResult> forgetPassword(string email)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return BadRequest(new ControllerHandleError(400, "email Is Not Reqistered!!"));
			}



			var TokenPasswordReset = await userManager.GeneratePasswordResetTokenAsync(user);

			accountRepo.sendEmail(email, "Reset Password", "Account/isValidPassLink", user, TokenPasswordReset);

			// هحفظ التوكين ف ال داتا بيز 
			//user.PassToken = TokenPasswordReset;
			await userManager.UpdateAsync(user);
			return Ok(new { message = "Sent Email Successfully!!" });


		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			// اعمل اتشك الاول ان الاميل كونفيرم ولا لا 
			var user = await userManager.FindByEmailAsync(loginDto.Email);
			if (user == null)
			{
				return BadRequest(error: new ControllerHandleError(400, "Email Isn`t Exists"));
			}
			if (!await userManager.IsEmailConfirmedAsync(user))
			{
				var TokenEmail = await userManager.GenerateEmailConfirmationTokenAsync(user);

					accountRepo.sendEmail(user.Email!, Subject: "ConFirm Email", "Account/SendEmail", user, TokenEmail);
			
				return BadRequest(error: new ControllerHandleError(400, "Email Isn`t Confirmed, Check Your Inbox To Confirm Your Email"));
			}
			var isPasswordSuccessed = await userManager.CheckPasswordAsync(user, loginDto.Password);
			if (!isPasswordSuccessed)
			{
				return BadRequest(error: new ControllerHandleError(400, "PassWord Isn`t Correct"));
			}
			////////// كده بقي اعمل توكين واحفظه ف ال كوكيز 
			var token = accountRepo.GenerateToken(user);
			// احفظه ف الكوكيز بقي وبعدها ظبط الكوكيز ف ال APP

			//Response.Cookies=>Response بقول للمتصفح يحفظ كذا بعيد عن الريكويست خالص
			Response.Cookies.Append("AuthCookie", token, new CookieOptions
			{
				HttpOnly = true,                 // مش هتكون قابلة للوصول من JS
				Secure = true,                   // HTTPS فقط
				SameSite = SameSiteMode.None, // لازم None علشان الكوكي تتبعت بين البورتات
				Expires = loginDto.saveMe?DateTimeOffset.UtcNow.AddDays(7):
				null //   // مفيش Expires → يبقى session cookie // يعني هيتحفظ بير سيشن زي م انا كنت عاوز 
				// صلاحية الكوكيز وبعدها هيتمسح 
				
				//,Path = "/" // الباس اللي جايلي ف الريكويست يعني لازم يكون كده وده الديفولت

				
			});

			return Ok(new { message ="Successfully Login!!"});


		}

		[HttpPost("logout")]
		public void logout()
		{
			//Response.Cookies=>Response معناها اني بقول لل متصفح يمسح كذا بعيد عن ال ريكويست اللي جاي خالص 
			Response.Cookies.Delete("AuthCookie", new CookieOptions
			{
				// لازم امسحه بنفس الاعدادات بتاعت الاضافه 
				//Path = "/",
				HttpOnly = true,
				SameSite = SameSiteMode.None, // لازم None علشان الكوكي تتبعت بين البورتات

				 Secure = true
				// لو كنت فعلته وقت الإنشاء، لازم تضيفه برضه هنا

			});
		}

		[HttpGet("isAuthorized")]
		public ActionResult<bool> IsAuthorized()
		{
			return Ok(Request.Cookies.ContainsKey("AuthCookie"));
		}


	}
}
