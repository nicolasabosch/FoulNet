using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Collections.Specialized;
using FoulNet.Model;
using CabernetDBContext;

namespace FoulNet.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Entities db;
        public UserController(Entities context)
        {
            db = context;
        }


        [Route("AllUser")]
        [HttpGet]
        public ActionResult AllUser()
        {
                        
            var list = (
                from User in db.User
                select new
                {
                User.UserID,  User.UserName }

            );
            
            var ret = list.AsEnumerable();
            return Ok(ret);
        }


        // GET api/User
        [HttpGet]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            
            var list = (
                from User in db.User
                select new
                {
                User.UserID, User.LogonName, User.UserName,  User.Active, User.Email }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.UserID == key);
            }

            if (parameters["UserID"] != null)
            {
                string userID = parameters["UserID"];
                list = list.Where(l => l.UserID == userID);
            }

            if (parameters["UserName"] != null)
            {
                string userName = parameters["UserName"];
                list = list.Where(l => l.UserName.Contains(userName));
                list = list.OrderBy(l => l.UserName.IndexOf(userName));
            }

            
            if (parameters["Email"] != null)
            {
                string email = parameters["Email"];
                list = list.Where(l => l.Email.Contains(email));
                
            }

            var ret = list.AsEnumerable();
            return Ok(ret);
        }

        // GET api/User/5
        [HttpGet("{id}")]
        public ActionResult GetUser(string id)
        {
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            var user = (
                from User in db.User
                where User.UserID == id
                select new
                {
                User.UserID, User.LogonName, User.Password, User.UserName,  User.Active, User.ForceChangePassword, User.Email, User.LastLogon, User.ReceiveNotification, User.CreatedOn, User.CreatedBy, User.LastModifiedOn, User.LastModifiedBy
                }

            ).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            dynamic record = user.ToExpando();
            record.UserRole = (
                from UserRole in db.UserRole
                where UserRole.UserID == id
                select new
                {
                UserRole.UserID, UserRole.RoleID, EntityStatus = "U"
                }

            ).ToList();
            return Ok(record);
        }

        // PUT api/User/5
        [HttpPut("{id}")]
        public ActionResult PutUser(string id, User user)
        {
            ModelState.Clear();
            Extensions.ClearReferences(user);
            TryValidateModel(user);
            if (ModelState.IsValid && id == user.UserID)
            {
                db.Entry(user).State = EntityState.Modified;
                Entities.ProcessChildrenUpdate(db, user.UserRole.ToList());
                Entities.ProcessChildrenUpdate(db, user.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                return NoContent();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // POST api/User
        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {
            ModelState.Clear();
            Extensions.ClearReferences(user);
            TryValidateModel(user);
            if (ModelState.IsValid)
            {
                db.User.Add(user);
                Entities.ProcessChildrenUpdate(db, user.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (UserExists(user.UserID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetUser", new
                {
                id = user.UserID
                }

                , user);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/User/5
        [HttpDelete("{id}")]
        public ActionResult<User> DeleteUser(string id)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.User.Remove(user);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return user;
        }


        [Route("Referees")]
        [HttpGet]
        public ActionResult Referees()
        {

            var list = (
                from User in db.User
                join UserRole in db.UserRole on User.UserID equals UserRole.UserID
                where UserRole.RoleID == "REFEREE"
                select new
                {
                    User.UserID,
                    User.UserName
                }

            );

            var ret = list.AsEnumerable();
            return Ok(ret);
        }

        private bool UserExists(string id)
        {
            return db.User.Any(e => e.UserID == id);
        }
    }
}