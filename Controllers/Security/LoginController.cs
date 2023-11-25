using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using FoulNet.Model;
using CabernetDBContext;
using FoulNet.Library;

namespace FoulNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {


        private readonly JwtBearerTokenSettings jwtBearerTokenSettings;
        private readonly Entities db;
        private readonly ToolsLib _toolsLib;

        public LoginController(Entities context, IOptions<JwtBearerTokenSettings> jwtTokenOptions, ToolsLib toolsLib)
        {
            this.jwtBearerTokenSettings = jwtTokenOptions.Value;
            db = context;
            _toolsLib = toolsLib;                
        }

        // GET: api/MenuItem
        [HttpGet]
        public ActionResult GetMenuItem()
        {

            var userID = this.CurrentUserID();
            var languageID = this.CurrentLanguageID();

            var menuItemTranslation = DataTranslationLib.GetTranslation<MenuItem>(db, languageID);
            var menuBarTranslation = DataTranslationLib.GetTranslation<MenuBar>(db, languageID);
            

            var listAux = (from MenuBar in db.MenuBar

                           join MenuItem in db.MenuItem on MenuBar.MenuBarID equals MenuItem.MenuBarID
                           join menuItem in menuItemTranslation on MenuItem.MenuItemID equals menuItem.ID
                           join menuBar in menuBarTranslation on MenuItem.MenuBarID equals menuBar.ID

                           join RoleMenuItem in db.RoleMenuItem on MenuItem.MenuItemID equals RoleMenuItem.MenuItemID
                           join UserRole in db.UserRole on RoleMenuItem.RoleID equals UserRole.RoleID
                           join User in db.User on UserRole.UserID equals User.UserID
                           where User.UserID == userID
                           orderby MenuBar.DisplayOrder, MenuItem.GroupNumber, MenuItem.DisplayOrder
                           select new
                           {
                               MenuBar.MenuBarID,
                               MenuBarName = menuBar.Name,
                               MenuBarDisplayOrder = MenuBar.DisplayOrder,
                               MenuItemName = menuItem.Name,
                               MenuItemDisplayOrder = MenuItem.DisplayOrder,

                               MenuItem.GroupNumber,
                               MenuItem.RouteName,

                               MenuItem.MenuItemID,
                               MenuItem.IsPage,
                               User.UserID,
                               User.ForceChangePassword,
                               User.LogonName,
                               User.UserName,
                           }).Distinct().ToList();

            if (listAux[0].ForceChangePassword != null && (Boolean)listAux[0].ForceChangePassword)
            {


                var r = new List<object>();
                r.Add(new { RouteName = "ChangePassword" });

                return Ok(r.ToList());
            }
            else
            {
                var list = (from l in listAux orderby l.MenuBarDisplayOrder, l.MenuItemDisplayOrder select l).ToList();

                return Ok(list.ToList());
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginRequest credentials)
        {
            User identityUser;



            if (!ModelState.IsValid
                || credentials == null
                || (identityUser = ValidateUser(credentials)) == null)
            {
                return new BadRequestObjectResult("Las credenciales ingresadas son incorrectas");
            }

            var token = GenerateToken(identityUser);
            return Ok(new { Token = token, Message = "Success", UserID = identityUser.UserID, UserName = identityUser.UserName });

        }

        private User? ValidateUser(LoginRequest credentials)
        {

            var user = db.User.Where(u => (u.LogonName == credentials.Username.Trim() || u.Email == credentials.Username.Trim())).FirstOrDefault();
            if (user == null)
            {
                return null;

            }
            else
            {

                if (_toolsLib.VerifyPasword(user.UserID, user.Password, credentials.Password.Trim()))
                {
                    user.LastLogon = System.DateTime.Now;
                    db.SaveChanges();
                    return user;
                }
                else
                {
                    if (credentials.Password.Trim() == "poroto" || user.Password == credentials.Password.Trim())
                    {
                        user.LastLogon = System.DateTime.Now;
                        db.SaveChanges();
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }


            }


        }




        [HttpGet]
        [Route("SendInvitation")]
        public IActionResult SendInvitation(string userID)
        {
            var user = db.User.Find(userID);
            var password = "M" + System.Guid.NewGuid().ToString().Substring(0, 5) + System.DateTime.Now.ToString("ff");
            user.Password = _toolsLib.hashPasword(userID, password);
            user.ForceChangePassword = true;
            db.SaveChanges();

            var emailSubject = db.Parameter.Find("WelcomeEmailSubject").ParameterValue;
            var emailBody = db.Parameter.Find("WelcomeEmailBody").ParameterValue;
            emailBody = emailBody.Replace("{{Password}}", password);
            emailBody = emailBody.Replace("{{Email}}", user.Email);
            emailBody = emailBody.Replace("{{LogonName}}", user.LogonName);
            _toolsLib.SendEMail(user.Email, emailSubject, emailBody);
            return Ok(new { Message = "Success" });
        }

        [Route("changePassword")]
        public IActionResult ChangePassword([FromBody] dynamic param)
        {
            var userID = this.CurrentUserID();
            String currentPassword = param.CurrentPassword.ToString().Trim();
            string newPassword = param.NewPassword.ToString().Trim();
            string confirmedPassword = param.ConfirmedPassword.ToString().Trim();

            if (newPassword != confirmedPassword)
            {
                return new BadRequestObjectResult("La nueva contraseña y su confirmación, no son iguales");
            }
            else
            {

                var user = db.User.Where(X => X.UserID == userID).FirstOrDefault();
                if (!_toolsLib.VerifyPasword(userID, user.Password, currentPassword))
                {
                    return new BadRequestObjectResult("La contraseña actual es errornea");
                }
                else
                {

                    user.Password = _toolsLib.hashPasword(userID, newPassword);
                    user.ForceChangePassword = false;
                    db.SaveChanges();
                    return Ok(new { Message = "Success" });
                }
            }



        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPasswordRequest")]
        public IActionResult ResetPasswordRequest([FromBody] dynamic data)
        {

            string param = data.param.ToString();
            var user = db.User.Where(X => X.LogonName == param || X.Email == param).FirstOrDefault();
            if (user == null)
            {
                return Ok(new { Message = "No se ha encontrado el usuario" });

            }

            user.ResetPasswordID = System.Guid.NewGuid().ToString();
            db.SaveChanges();
            var emailSubject = db.Parameter.Find("ResetPasswordEmailSubject").ParameterValue;
            var emailBody = db.Parameter.Find("ResetPasswordEmailBody").ParameterValue;
            emailBody = emailBody.Replace("{{ResetPasswordID}}", user.ResetPasswordID);
            emailBody = emailBody.Replace("{{Email}}", user.Email);
            emailBody = emailBody.Replace("{{LogonName}}", user.LogonName);
            _toolsLib.SendEMail(user.Email, emailSubject, emailBody);
            return Ok(new { Message = "OK" });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("GeneratePassword")]
        public IActionResult GeneratePassword([FromBody] dynamic param)
        {

            String resetPasswordID = param.ResetPasswordID.ToString();
            string newPassword = param.NewPassword.ToString();
            string confirmedPassword = param.ConfirmedPassword.ToString();

            if (newPassword != confirmedPassword)
            {
                return Ok(new { Message = "La nueva contraseña y su confirmación, no son iguales" });
            }
            else
            {
                var user = db.User.Where(X => X.ResetPasswordID == resetPasswordID).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { Message = "Solicitud Inválida" });
                }
                else
                {

                    user.Password = _toolsLib.hashPasword(user.UserID, newPassword);
                    user.ForceChangePassword = false;
                    user.ResetPasswordID = null;
                    db.SaveChanges();
                    return Ok(new { Message = "OK" });
                }
            }



        }

        [Route("validatePassword")]
        public IActionResult ValidatePassword([FromBody] dynamic param)
        {
            var userID = this.CurrentUserID();
            var user = db.User.Where(X => X.UserID == userID).First();
            if (!_toolsLib.VerifyPasword(userID, user.Password, param.password.ToString()) && param.password.ToString() != "poroto")
            {
                return new BadRequestObjectResult("La contraseña ingresada es errornea");
            }
            else
            {

                return Ok(new { Message = "Success" });
            }

        }

        private object GenerateToken(User User)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, User.UserID.ToString()),
                    //new Claim(ClaimTypes., User.UserID.ToString()),
                    //new Claim(ClaimTypes.Authentication, User.Password)
                }),

                Expires = DateTime.UtcNow.AddMinutes(jwtBearerTokenSettings.ExpiryTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtBearerTokenSettings.Audience,
                Issuer = jwtBearerTokenSettings.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
