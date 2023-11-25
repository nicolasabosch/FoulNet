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
using Microsoft.AspNetCore.Authorization;

namespace FoulNet.Controllers
{
    [Route("api/Language")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly Entities db;
        public LanguageController(Entities context)
        {
            db = context;
        }

        // GET api/Language
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
         //   var languageTranslation = DataTranslationLib.GetTranslation<Language>(db, languageID);
            var list = (
                from Language in db.Language
                //join language in languageTranslation on Language.LanguageID equals language.ID
                select new
                {
                    Language.LanguageID,
                    Language.LanguageName
                    //LanguageName = language.Name, }
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.LanguageID == key);
            }

            if (parameters["LanguageID"] != null)
            {
                string lID = parameters["LanguageID"];
                list = list.Where(l => l.LanguageID == lID);
            }

            if (parameters["LanguageName"] != null && parameters["LanguageFullName"] == null)
            {
                string languageName = parameters["LanguageName"];
                list = list.Where(l => l.LanguageName.Contains(languageName));
                list = list.OrderBy(l => l.LanguageName.IndexOf(languageName));
            }

            var ret = list.ToList();
            return Ok(ret);
        }

        // GET api/Language/5
        [HttpGet("{id}")]
        public ActionResult GetLanguage(string id)
        {
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            var language = (
                from Language in db.Language
                where Language.LanguageID == id
                select new
                {
                Language.LanguageID, Language.LanguageName, Language.CreatedOn, Language.CreatedBy, Language.LastModifiedOn, Language.LastModifiedBy
                }

            ).FirstOrDefault();
            if (language == null)
            {
                return NotFound();
            }

            dynamic record = language.ToExpando();
            return Ok(record);
        }

        // PUT api/Language/5
        [HttpPut("{id}")]
        public ActionResult PutLanguage(string id, Language language)
        {
            ModelState.Clear();
            Extensions.ClearReferences(language);
            TryValidateModel(language);
            if (ModelState.IsValid && id == language.LanguageID)
            {
                db.Entry(language).State = EntityState.Modified;
                Entities.ProcessChildrenUpdate(db, language.DataTranslation.ToList());
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

        // POST api/Language
        [HttpPost]
        public ActionResult<Language> PostLanguage(Language language)
        {
            ModelState.Clear();
            Extensions.ClearReferences(language);
            TryValidateModel(language);
            if (ModelState.IsValid)
            {
                db.Language.Add(language);
                Entities.ProcessChildrenUpdate(db, language.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (LanguageExists(language.LanguageID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetLanguage", new
                {
                id = language.LanguageID
                }

                , language);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/Language/5
        [HttpDelete("{id}")]
        public ActionResult<Language> DeleteLanguage(string id)
        {
            Language language = db.Language.Find(id);
            if (language == null)
            {
                return NotFound();
            }

            db.Language.Remove(language);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return language;
        }

        private bool LanguageExists(string id)
        {
            return db.Language.Any(e => e.LanguageID == id);
        }
    }
}