using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ANBIDENTITYSERVICE.Models;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PasswordWebApp.Models;

namespace ANBIDENTITYSERVICE.Controllers
{
    [ApiController]
    [EnableCors("AllowMyOrign")]
    [Route("api/[controller]")]
    public class UserRegistrationController : ControllerBase
    {
        

        private readonly ILogger<UserRegistrationController> _logger;
        private UserManager<AppUser> userMgr;
        private SignInManager<AppUser> siginMgr;
        public UserRegistrationController(ILogger<UserRegistrationController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            userMgr = userManager;
            siginMgr = signInManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(AppUser appUser)
        {
            Message _Message = new Message();
            try
            {
                AppUser user = await userMgr.FindByEmailAsync(appUser.Email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = appUser.UserName,
                        Email = appUser.Email,
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName
                    };
                    IdentityResult result = await userMgr.CreateAsync(user, "Test123!");
                    if (result.Succeeded == true)
                    {
                        // string callbackUrl = "file://C:/Users/sriram.biccavolu/Desktop/resetform.html";
                        string callbackUrl = "https://localhost:44325/Home/Privacy?IsSuperUser=" + appUser.Email;

                        var user1 = await userMgr.FindByEmailAsync(appUser.Email);
                        //var code = await userMgr.GeneratePasswordResetTokenAsync(user1);
                        var subject = "Reset password request.";
                        var body = "<html><body>" +
                                   $"<h2>Hi {user1.FirstName.ToUpper().ToString()}!</h2>" +
                                   //"<h1>Please reset your password</h1>" +
                                   $"<a  href=\"" + callbackUrl + "\"> please click this link to reset your password </a>" +
                        "</body></html>";
                        var message = new IdentityMessage
                        {
                            Body = body,
                            Destination = "sriram.biccavolu@anblicks.com",
                            Subject = subject
                        };
                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("sriram.biccavolu@anblicks.com");
                        msg.To.Add(new MailAddress(message.Destination));
                        msg.Subject = message.Subject;
                        msg.Body = message.Body;
                        msg.IsBodyHtml = true;
                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587))
                        {
                            UseDefaultCredentials = false
                        };
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("sriram.biccavolu@anblicks.com", "sriram@123");
                        smtpClient.Credentials = credentials;
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(msg);
                        smtpClient.Dispose();
                        _Message.Status = "Success";
                        _Message.StatusCode = 200;
                        _Message.StatusMessage = "User Successfully Created !!";
                    }
                    else
                    {
                        _Message.Status = "Failure";
                        _Message.StatusCode = 200;
                        _Message.StatusMessage = "User Already Existed !!";
                    }
                }
            }
            catch (Exception ex)
            {
                _Message.Status = "Failure";
                _Message.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                _Message.StatusMessage = ex.Message;
            }
            return Ok(_Message);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword resetPassword)
        {
            Message _Message = new Message();
            IdentityResult identityResult = new IdentityResult();

            try
            {
                AppUser user = await userMgr.FindByEmailAsync(resetPassword.email);
                var token = await userMgr.GeneratePasswordResetTokenAsync(user);
                //var encodedtoken = Encoding.UTF8.GetBytes(token);
                //var validatetoken = WebEncoders.Base64UrlEncode(encodedtoken);
                if (user != null && user.Email == resetPassword.email)
                {
                    identityResult = await userMgr.ResetPasswordAsync(user, token, resetPassword.passwordConfirm);
                    if (identityResult.Succeeded == true)
                    {
                        _Message.Status = "Success";
                        _Message.StatusCode = 200;
                        _Message.StatusMessage = "Successfully your Passwors Reset !!";
                    }

                    return Ok(_Message);
                }
                else
                {
                    _Message.Status = "Failure";
                    _Message.StatusCode = 200;
                    _Message.StatusMessage = "Passwors Reset Failed !!";
                    return Ok(_Message);
                }
            }
            catch (Exception ex)
            {
                _Message.Status = "Failure";
                _Message.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                _Message.StatusMessage = ex.Message;
            }
            return Ok(identityResult);
        }



        [HttpPost("WorkHomeRequest")]
        public IActionResult WorkHomeRequest([FromBody] WorkHomeRequestModel workHomeRequestModel)
        {
            return Ok();
        }





        }
}
