using CoreLayer.Handle_Exception;
using CoreLayer.Models.UserModel;
using DnsClient;
using Full_E_Commerce_Project.Handle_MiddleWares;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Z.Expressions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace RepositoryLayer.Account
{
	public class AccountRepo
	{
		private readonly IConfiguration config;
		private readonly UserManager<AppUser> userManager;

		public AccountRepo(IConfiguration config, UserManager<AppUser> userManager)
		{
			this.config = config;
			this.userManager = userManager;
		}
		private bool IsValidEmailFormat(string email)
		{
			var emailAttr = new EmailAddressAttribute();
			return emailAttr.IsValid(email);
		}

		private bool HasMxRecord(string domain)
		{
			var lookup = new LookupClient();
			var result = lookup.Query(domain, QueryType.MX); //MX=>Mail Exchange records
			return result.Answers.Any();
		}
		public void sendEmail(string To, string Subject, string Component, AppUser user, string TokenEmail)
		{
			//In This Class We Will Use Method SendEmail That Have Parameters(To, Subject, Component, email, Token-Email)
			//And In This Method It Will Send Email With Link That Redirect To Component With Email And Token-Email

			// 1- تحقق من Format
			if (!IsValidEmailFormat(To))
				throw new MyException(400, "❌ Email format is invalid!");

			// 2- تحقق من MX Record
			var domain = To.Split('@').Last();
			if (!HasMxRecord(domain))
				throw new MyException(400, "❌ Email domain not found!");


			var EmailBody = @$"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>Email Template</title>
</head>
<body style=""margin:0; padding:0; font-family: Arial, sans-serif; background-color:#f4f4f4;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f4f4; padding:20px 0;"">
    <tr>
      <td align=""center"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);"">
          <!-- Header -->
          <tr>
            <td align=""center"" style=""background-color:#4CAF50; padding:20px;"">
              <h1 style=""color:#ffffff; margin:0; font-size:24px;"">Welcome to Our Service 👋</h1>
            </td>
          </tr>
          <!-- Body -->
          <tr>
            <td style=""padding:30px; color:#333333; font-size:16px; line-height:1.6;"">
              <p>Hello <strong>{user.FirstName+" "+user.LastName}</strong>,</p>
              <p>Please {Subject} by clicking the button below:</p>
              <p style=""text-align:center; margin:30px 0;"">
                <a href=""{config["Gmail:FrontEndUrl"]+Component+"?email="+user.Email+"&token="+TokenEmail}""
                   style=""background-color:#4CAF50; color:#ffffff; text-decoration:none; padding:12px 24px; border-radius:6px; display:inline-block; font-size:16px;"">
                  ✅ {Subject}
                </a>
              </p>
              <p>If you didn’t create this account, please ignore this email.</p>
              <p style=""margin-top:30px;"">Best regards,<br><strong>The Support Team</strong></p>
            </td>
          </tr>
          <!-- Footer -->
          <tr>
            <td align=""center"" style=""background-color:#f4f4f4; color:#777777; font-size:12px; padding:15px;"">
              © 2025 E-Commerce. All rights reserved.
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";


            //message.From.Add (new MailboxAddress ("Joey", "joey@friends.com"));
            var message = new MimeMessage()
            {
                From = { new MailboxAddress("E-Commerce", config["Gmail:From"]) },
                To = { new MailboxAddress(user.FirstName + " " + user.LastName, To) },
                Subject = Subject,
                Body = new TextPart("html")
                {
                    Text = EmailBody
                }
            };

			using (var smtp = new MailKit.Net.Smtp.SmtpClient())
			{
                smtp.Connect(config["Gmail:Smtp"], int.Parse(config["Gmail:Port"]!), true);

				// Note: only needed if the SMTP server requires authentication
				smtp.Authenticate(config["Gmail:From"], config["Gmail:Pass"]);
				smtp.Send(message);
                
				smtp.Disconnect(true);
			};


		}
		public async Task VerifyEmailAsync(string email, string Token)
        {
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				throw new MyException(400, "email Is Not Reqistered!!");

			}
			var isConfirmed = await userManager.IsEmailConfirmedAsync(user);
			if (isConfirmed == true)
			{
				throw new MyException(400, "Already Confirmed!!");

			}
			var decodedToken = WebUtility.UrlDecode(Token).Replace(" ", "+"); // علشان يعرف انه جاي من ال يو ار ال ف يهندله ع الاساس ده 


			var checkVaildToken = await userManager.ConfirmEmailAsync(user, decodedToken);
			if (checkVaildToken.Succeeded)
			{
                return;
			}
			List<string> Errors = checkVaildToken.Errors.Select(e => e.Description).ToList();
			throw new MyException(400, "Confirmation failed, please Try Login Again To Send  a new Configration Email", develoberMessages: Errors);
		}

		public string GenerateToken(AppUser user)
		{

			// JWT => JSON WEB TOKEN
			//	public JwtSecurityToken
			//	(string issuer = null, string audience = null, IEnumerable<Claim> claims = null, DateTime? notBefore = null,
			//	DateTime? expires = null, SigningCredentials signingCredentials = null)

			string issuer = config["JWT:Issuer"]!;
			string audience = config["JWT:Audience"]!;
			List<Claim> claims = new()
			{
				new Claim(ClaimTypes.NameIdentifier,user.Id),
				new Claim(ClaimTypes.Email,user.Email!),
				new Claim(ClaimTypes.Name,user.UserName !),
				new Claim(type: Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Claim فريد لكل توكن //Jti يمنع استخدام نفس التوكن أكتر من مرة (replay attack) لو حبيت تعمل Revocation.

			};
			DateTime? notBefore = DateTime.UtcNow;
			int DurationInDays = int.Parse(config["JWT:DurationInDays"]!);
			DateTime? expires = DateTime.UtcNow.AddDays(DurationInDays);
			//	public SigningCredentials(SecurityKey key, string algorithm)

			SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
			string algorithm = SecurityAlgorithms.HmacSha256;
			SigningCredentials signingCredentials = new SigningCredentials(key, algorithm);


			var jwtToken = new JwtSecurityToken(issuer, audience, claims, notBefore, expires, signingCredentials);

			var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			return token;
		}

	}
}
