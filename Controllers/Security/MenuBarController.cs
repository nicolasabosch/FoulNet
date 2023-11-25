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
    [Route("api/MenuBar")]
    [ApiController]
    public class MenuBarController : ControllerBase
    {
        private readonly Entities db;
        public MenuBarController(Entities context)
        {
            db = context;
        }

        // GET api/MenuBar
        [HttpGet]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            var menuBarTranslation = DataTranslationLib.GetTranslation<Model.MenuBar>(db, languageID);
            var list = (
                from MenuBar in db.MenuBar
                join menuBar in menuBarTranslation on MenuBar.MenuBarID equals menuBar.ID
                select new
                {
                MenuBar.MenuBarID, MenuBarName = menuBar.Name, MenuBar.DisplayOrder, }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.MenuBarID == key);
            }

            if (parameters["MenuBarID"] != null)
            {
                string menuBarID = parameters["MenuBarID"];
                list = list.Where(l => l.MenuBarID == menuBarID);
            }

            if (parameters["MenuBarName"] != null && parameters["MenuBarFullName"] == null)
            {
                string menuBarName = parameters["MenuBarName"];
                list = list.Where(l => l.MenuBarName.Contains(menuBarName));
                list = list.OrderBy(l => l.MenuBarName.IndexOf(menuBarName));
            }

            var ret = list.AsEnumerable();
            return Ok(ret);
        }

        // GET api/MenuBar/5
        [HttpGet("{id}")]
        public ActionResult GetMenuBar(string id)
        {
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            var menuBar = (
                from MenuBar in db.MenuBar
                where MenuBar.MenuBarID == id
                select new
                {
                MenuBar.MenuBarID, MenuBar.MenuBarName, MenuBar.DisplayOrder, MenuBar.CreatedOn, MenuBar.CreatedBy, MenuBar.LastModifiedOn, MenuBar.LastModifiedBy
                }

            ).FirstOrDefault();
            if (menuBar == null)
            {
                return NotFound();
            }

            dynamic record = menuBar.ToExpando();
            return Ok(record);
        }

        // PUT api/MenuBar/5
        [HttpPut("{id}")]
        public ActionResult PutMenuBar(string id, MenuBar menuBar)
        {
            ModelState.Clear();
            Extensions.ClearReferences(menuBar);
            TryValidateModel(menuBar);
            if (ModelState.IsValid && id == menuBar.MenuBarID)
            {
                db.Entry(menuBar).State = EntityState.Modified;
                Entities.ProcessChildrenUpdate(db, menuBar.DataTranslation.ToList());
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

        // POST api/MenuBar
        [HttpPost]
        public ActionResult<MenuBar> PostMenuBar(MenuBar menuBar)
        {
            ModelState.Clear();
            Extensions.ClearReferences(menuBar);
            TryValidateModel(menuBar);
            if (ModelState.IsValid)
            {
                db.MenuBar.Add(menuBar);
                Entities.ProcessChildrenUpdate(db, menuBar.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (MenuBarExists(menuBar.MenuBarID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetMenuBar", new
                {
                id = menuBar.MenuBarID
                }

                , menuBar);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/MenuBar/5
        [HttpDelete("{id}")]
        public ActionResult<MenuBar> DeleteMenuBar(string id)
        {
            MenuBar menuBar = db.MenuBar.Find(id);
            if (menuBar == null)
            {
                return NotFound();
            }

            db.MenuBar.Remove(menuBar);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return menuBar;
        }

        private bool MenuBarExists(string id)
        {
            return db.MenuBar.Any(e => e.MenuBarID == id);
        }
    }
}