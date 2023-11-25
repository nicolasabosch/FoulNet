using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FoulNet.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CabernetDBContext;

namespace FoulNet.Controllers
{
    [Route("api/TextTranslation")]
    [ApiController]
    public class TextTranslationController : ControllerBase
    {
        private readonly Entities db;
        public TextTranslationController(Entities context)
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
            var list = (from TextTranslation in db.TextTranslation
                        where TextTranslation.LanguageID == languageID
                        orderby TextTranslation.Text
                        select new
                        {
                            TextTranslation.LanguageID,
                            TextTranslation.Text,
                            TextTranslation.Translation
                        });
            var ret = list.AsEnumerable();
            return Ok(ret);
        }


        [HttpGet("{id}")]
        public ActionResult GetTextTranslation(string id)
        {
            var languageID = this.CurrentLanguageID();
            var list = (from TextTranslation in db.TextTranslation
                        from U in db.TextTranslation.Where(X => X.Text == TextTranslation.Text && X.LanguageID == languageID).DefaultIfEmpty()
                        from T in db.TextTranslation.Where(X => X.Text == TextTranslation.Text && X.LanguageID == id).DefaultIfEmpty()
                        where TextTranslation.LanguageID == languageID
                        orderby TextTranslation.Text
                        select new
                        {
                            TextTranslation.LanguageID,
                            TextTranslation.Text,
                            OriginalTranslation = T.Translation,
                            DisplayText= U == null?TextTranslation.Text:U.Translation,
                            T.Translation,
                            Current = languageID,
                            EntityStatus = T == null ? "A" : "U",
                            T.LastModifiedOn
                        });


            var ret = list.AsEnumerable();
            return Ok(ret);

        }

        // PUT api/Language/5
        [HttpPut()]
        public ActionResult PutTextTranslation(TextTranslation textTranslation)
        {

            Extensions.ClearReferences(textTranslation);

            if (ModelState.IsValid)
            {
                db.Entry(textTranslation).State = EntityState.Modified;


                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                return Ok(textTranslation);
            }
            else
            {
                return BadRequest(ModelState);
            }



        }

        // POST api/Language
        [HttpPost]
        [AllowAnonymous]
        public ActionResult PostTextTranslation(TextTranslation textTranslation)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.TextTranslation.Add(textTranslation);
                    db.SaveChanges();
                }
                catch
                {
                }

                return Ok(textTranslation);
            }

            else
            {
                return BadRequest(ModelState);

            }

        }


    }
}