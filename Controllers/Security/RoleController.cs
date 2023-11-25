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
    [Route("api/Role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly Entities db;
        public RoleController(Entities context)
        {
            db = context;
        }

        // GET api/Role
        [HttpGet]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            
            var list = (
                from Role in db.Role
            select new
                {
                Role.RoleID, Role.RoleName}

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.RoleID == key);
            }

            if (parameters["RoleID"] != null)
            {
                string roleID = parameters["RoleID"];
                list = list.Where(l => l.RoleID == roleID);
            }

            if (parameters["RoleName"] != null && parameters["RoleFullName"] == null)
            {
                string roleName = parameters["RoleName"];
                list = list.Where(l => l.RoleName.Contains(roleName));
                list = list.OrderBy(l => l.RoleName.IndexOf(roleName));
            }

            
            var ret = list.AsEnumerable();
            return Ok(ret);
        }

        // GET api/Role/5
        [HttpGet("{id}")]
        public ActionResult GetRole(string id)
        {
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            var role = (
                from Role in db.Role
                where Role.RoleID == id
                select new
                {
                Role.RoleID, Role.RoleName, Role.CreatedOn, Role.CreatedBy, Role.LastModifiedOn, Role.LastModifiedBy
                }

            ).FirstOrDefault();
            if (role == null)
            {
                return NotFound();
            }

            dynamic record = role.ToExpando();
            record.RoleMenuItem = (
                from RoleMenuItem in db.RoleMenuItem
                where RoleMenuItem.RoleID == id
                select new
                {
                RoleMenuItem.RoleID, RoleMenuItem.MenuItemID, EntityStatus = "U"
                }

            ).ToList();
            return Ok(record);
        }

        // PUT api/Role/5
        [HttpPut("{id}")]
        public ActionResult PutRole(string id, Role role)
        {
            ModelState.Clear();
            Extensions.ClearReferences(role);
            TryValidateModel(role);
            if (ModelState.IsValid && id == role.RoleID)
            {
                db.Entry(role).State = EntityState.Modified;
                Entities.ProcessChildrenUpdate(db, role.RoleMenuItem.ToList());
                Entities.ProcessChildrenUpdate(db, role.DataTranslation.ToList());
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

        // POST api/Role
        [HttpPost]
        public ActionResult<Role> PostRole(Role role)
        {
            ModelState.Clear();
            Extensions.ClearReferences(role);
            TryValidateModel(role);
            if (ModelState.IsValid)
            {
                db.Role.Add(role);
                Entities.ProcessChildrenUpdate(db, role.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (RoleExists(role.RoleID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetRole", new
                {
                id = role.RoleID
                }

                , role);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/Role/5
        [HttpDelete("{id}")]
        public ActionResult<Role> DeleteRole(string id)
        {
            Role role = db.Role.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            db.Role.Remove(role);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return role;
        }

        private bool RoleExists(string id)
        {
            return db.Role.Any(e => e.RoleID == id);
        }
    }
}