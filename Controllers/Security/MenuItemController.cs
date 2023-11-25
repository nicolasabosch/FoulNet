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
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly Entities db;
        public MenuItemController(Entities context)
        {
            db = context;
        }

        // GET api/MenuItem
        [HttpGet]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };
            var menuItemTranslation = DataTranslationLib.GetTranslation<Model.MenuItem>(db, languageID);
            var menuBarTranslation = DataTranslationLib.GetTranslation<MenuBar>(db, languageID);
            

            var list = (
                from MenuItem in db.MenuItem
                join menuItem in menuItemTranslation on MenuItem.MenuItemID equals menuItem.ID
                join menuBar in menuBarTranslation on MenuItem.MenuBarID equals menuBar.ID
                
                select new
                {
                    MenuItem.MenuItemID,
                    MenuItemName = menuItem.Name,
                    MenuItem.MenuBarID,
                    MenuBarName = menuBar.Name,
                    MenuItem.RouteName,
                
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.MenuItemID == key);
            }

            if (parameters["MenuItemID"] != null)
            {
                string menuItemID = parameters["MenuItemID"];
                list = list.Where(l => l.MenuItemID == menuItemID);
            }

            if (parameters["MenuItemName"] != null && parameters["MenuItemFullName"] == null)
            {
                string menuItemName = parameters["MenuItemName"];
                list = list.Where(l => l.MenuItemName.Contains(menuItemName));
                list = list.OrderBy(l => l.MenuItemName.IndexOf(menuItemName));
            }

            if (parameters["MenuBarID"] != null)
            {
                string menuBarID = parameters["MenuBarID"];
                list = list.Where(l => l.MenuBarID == menuBarID);
            }

            if (parameters["RouteName"] != null)
            {
                string routeName = parameters["RouteName"];
                list = list.Where(l => l.RouteName.Contains(routeName));
                list = list.OrderBy(l => l.RouteName.IndexOf(routeName));
            }

            
            var ret = list.AsEnumerable();
            return Ok(ret);
        }

        // GET api/MenuItem/5

        [HttpGet("{id}")]
        public ActionResult GetMenuItem(string id)
        {
            var menuItem = db.MenuItem.Find(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            dynamic record = menuItem.ToExpando();

            record.Role = (from Role in db.Role
                           join RoleMenuItem in db.RoleMenuItem on Role.RoleID equals RoleMenuItem.RoleID
                           where RoleMenuItem.MenuItemID == id
                           select new { Role.RoleID, Role.RoleName }).ToList();

            record.User = (from Role in db.Role
                           join RoleMenuItem in db.RoleMenuItem on Role.RoleID equals RoleMenuItem.RoleID
                           join UserRole in db.UserRole on Role.RoleID equals UserRole.RoleID
                           join User in db.User on UserRole.UserID equals User.UserID
                           where RoleMenuItem.MenuItemID == id
                           orderby User.UserName,  Role.RoleName
                           select new
                           {
                               Role.RoleName,
                               User.UserName,
                               User.LogonName,
                               User.Active,
                           }).ToList();

            return Ok(record);

        }

        // PUT api/MenuItem/5
        [HttpPut("{id}")]
        public ActionResult PutMenuItem(string id, MenuItem menuItem)
        {
            ModelState.Clear();
            Extensions.ClearReferences(menuItem);
            TryValidateModel(menuItem);
            if (ModelState.IsValid && id == menuItem.MenuItemID)
            {
                db.Entry(menuItem).State = EntityState.Modified;
                Entities.ProcessChildrenUpdate(db, menuItem.DataTranslation.ToList());
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

        // POST api/MenuItem
        [HttpPost]
        public ActionResult<MenuItem> PostMenuItem(MenuItem menuItem)
        {
            ModelState.Clear();
            Extensions.ClearReferences(menuItem);
            TryValidateModel(menuItem);
            if (ModelState.IsValid)
            {
                db.MenuItem.Add(menuItem);
                Entities.ProcessChildrenUpdate(db, menuItem.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (MenuItemExists(menuItem.MenuItemID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetMenuItem", new
                {
                    id = menuItem.MenuItemID
                }

                , menuItem);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/MenuItem/5
        [HttpDelete("{id}")]
        public ActionResult<MenuItem> DeleteMenuItem(string id)
        {
            MenuItem menuItem = db.MenuItem.Find(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            db.MenuItem.Remove(menuItem);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return menuItem;
        }

        private bool MenuItemExists(string id)
        {
            return db.MenuItem.Any(e => e.MenuItemID == id);
        }
    }
}