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
    [Route("api/Parameter")]
    [ApiController]
    public class ParameterController : ControllerBase
    {
        private readonly Entities db;
        public ParameterController(Entities context)
        {
            db = context;
        }

        // GET api/Parameter
        [HttpGet]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            
            var list = (
                from Parameter in db.Parameter
                
                select new
                {
                Parameter.ParameterID, Parameter.ParameterName }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.ParameterID == key);
            }

            if (parameters["ParameterID"] != null)
            {
                string parameterID = parameters["ParameterID"];
                list = list.Where(l => l.ParameterID == parameterID);
            }

            if (parameters["ParameterName"] != null)
            {
                string parameterName = parameters["ParameterName"];
                list = list.Where(l => l.ParameterName.Contains(parameterName));
                list = list.OrderBy(l => l.ParameterName.IndexOf(parameterName));
            }

           

            var ret = list.AsEnumerable();
            return Ok(ret);
        }

        // GET api/Parameter/5
        [HttpGet("{id}")]
		[AllowAnonymous]
        public ActionResult GetParameter(string id)
        {
            var languageID = this.CurrentLanguageID();
            var previewList = new[]{".jpeg", ".jpg", ".png", ".gif", ".bmp"};
            var parameter = (
                from Parameter in db.Parameter
                where Parameter.ParameterID == id
                select new
                {
                Parameter.ParameterID, Parameter.ParameterName,  Parameter.ParameterDescription, Parameter.ParameterValue, Parameter.CreatedOn, Parameter.CreatedBy, Parameter.LastModifiedOn, Parameter.LastModifiedBy
                }

            ).FirstOrDefault();
            if (parameter == null)
            {
                return NotFound();
            }

            dynamic record = parameter.ToExpando();
            return Ok(record);
        }

        // PUT api/Parameter/5
        [HttpPut("{id}")]
        public ActionResult PutParameter(string id, Parameter parameter)
        {
            ModelState.Clear();
            Extensions.ClearReferences(parameter);
            TryValidateModel(parameter);
            if (ModelState.IsValid && id == parameter.ParameterID)
            {
                db.Entry(parameter).State = EntityState.Modified;
                Entities.ProcessChildrenUpdate(db, parameter.DataTranslation.ToList());
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

        // POST api/Parameter
        [HttpPost]
        public ActionResult<Parameter> PostParameter(Parameter parameter)
        {
            ModelState.Clear();
            Extensions.ClearReferences(parameter);
            TryValidateModel(parameter);
            if (ModelState.IsValid)
            {
                db.Parameter.Add(parameter);
                Entities.ProcessChildrenUpdate(db, parameter.DataTranslation.ToList());
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (ParameterExists(parameter.ParameterID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetParameter", new
                {
                id = parameter.ParameterID
                }

                , parameter);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/Parameter/5
        [HttpDelete("{id}")]
        public ActionResult<Parameter> DeleteParameter(string id)
        {
            Parameter parameter = db.Parameter.Find(id);
            if (parameter == null)
            {
                return NotFound();
            }

            db.Parameter.Remove(parameter);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return parameter;
        }

        private bool ParameterExists(string id)
        {
            return db.Parameter.Any(e => e.ParameterID == id);
        }
    }
}