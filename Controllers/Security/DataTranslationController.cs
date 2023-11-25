using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FoulNet.Model;
//using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabernetDBContext;
using Newtonsoft.Json;

namespace FoulNet.Controllers
{
    [Route("api/DataTranslation")]
    [ApiController]
    public class DataTranslationController : ControllerBase
    {
        private readonly Entities db;

        public DataTranslationController(Entities context)
        {
            db = context;

        }

        // GET api/Language
        [HttpGet]

        public ActionResult GetTranslation(string id, string fieldName)
        {
            var languageID = this.CurrentLanguageID();


            var languageTranslation = DataTranslationLib.GetTranslation<Language>(db, languageID);


            var list = (from Language in db.Language
                        join language in languageTranslation on Language.LanguageID equals language.ID
                        from DataTranslation in db.DataTranslation.Where(X => X.ID == id && X.FieldName == fieldName && X.LanguageID == Language.LanguageID).DefaultIfEmpty()
                        where Language.LanguageID != "ES"
                        select new
                        {
                            Language.LanguageID,
                            LanguageName = language.Name,
                            FieldName = fieldName,
                            ID = id,
                            DataTranslation.Translation,
                            EntityStatus = DataTranslation == null ? "A" : "U"
                        }).ToList();

            return Ok(list);

        }


        [HttpGet("GetDataTranslation")]
        public async Task<ActionResult<IEnumerable<object>>> GetDataTranslation(string dataTableID, string languageID)
        {
            var currentLanguageID = this.CurrentLanguageID();

            var originalList = await DataTranslationLib.GetTranslation(db, dataTableID, currentLanguageID).AsNoTracking().ToListAsync();
            var translatedList = await DataTranslationLib.GetTranslation(db, dataTableID, languageID).AsNoTracking().ToListAsync();



            var list = (from O in originalList
                        join T in translatedList on O.ID equals T.ID
                        orderby O.Name
                        select new
                        {
                            O.ID,
                            Text = O.Name,
                            Translation = T.Translated ? T.Name : "",
                            EntityStatus = T.Translated ? "U" : "A"

                        }).ToList();


            return Ok(list);


        }

        // PUT api/Language/5
        [HttpPut()]
        public ActionResult PutDataTranslation(DataTranslation dataTranslation)
        {

            this.ClearReferences(dataTranslation);

            if (ModelState.IsValid)
            {
                db.Entry(dataTranslation).State = EntityState.Modified;


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
        public ActionResult PostDataTranslation(DataTranslation textTranslation)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.DataTranslation.Add(textTranslation);
                    db.SaveChanges();
                }
                catch
                {
                }

                return Ok();
            }

            else
            {
                return BadRequest(ModelState);

            }

        }


        
    }
}