using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoulNet;
using FoulNet.Model;
using System.Collections.Specialized;
using System.Web;
using CabernetDBContext;

namespace FoulNet.Controllers.Administration
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTypeController : ControllerBase
    {
        private readonly Entities _context;

        public ProjectTypeController(Entities context)
        {
            _context = context;
        }

        // GET: api/ProjectType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectType>>> GetProjectType()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var projectTypeTranslation = DataTranslationLib.GetTranslation<ProjectType>(_context, languageID);

            var list = (
                from ProjectType in _context.ProjectType
                join projectType in projectTypeTranslation on ProjectType.ProjectTypeID equals projectType.ID
                select new
                {
                    ProjectType.ProjectTypeID,
                    ProjectTypeName = projectType.Name,
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.ProjectTypeID == key);
            }

            if (parameters["ProjectTypeID"] != null)
            {
                string projectTypeID = parameters["ProjectTypeID"];
                list = list.Where(l => l.ProjectTypeID == projectTypeID);
            }

            if (parameters["ProjectTypeName"] != null)
            {
                string projectTypeName = parameters["ProjectTypeName"];
                list = list.Where(l => l.ProjectTypeName.Contains(projectTypeName));
                list = list.OrderBy(l => l.ProjectTypeName.IndexOf(projectTypeName));
            }

            var ret = await list.ToListAsync();
            return Ok(ret);
        }

        // GET: api/ProjectType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectType>> GetProjectType(string id)
        {
            if (_context.ProjectType == null)
            {
                return NotFound();
            }
            var ProjectType = await _context.ProjectType.FindAsync(id);

            if (ProjectType == null)
            {
                return NotFound();
            }

            return ProjectType;
        }

        // PUT: api/ProjectType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectType(string id, ProjectType ProjectType)
        {
            if (id != ProjectType.ProjectTypeID)
            {
                return BadRequest();
            }

            _context.Entry(ProjectType).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(_context, ProjectType.DataTranslation.ToList());

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProjectType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectType>> PostProjectType(ProjectType ProjectType)
        {
            if (_context.ProjectType == null)
            {
                return Problem("Entity set 'Entities.ProjectType'  is null.");
            }
            _context.ProjectType.Add(ProjectType);
            Entities.ProcessChildrenUpdate(_context, ProjectType.DataTranslation.ToList());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectType", new { id = ProjectType.ProjectTypeID }, ProjectType);
        }

        // DELETE: api/ProjectType/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProjectType>> DeleteProjectType(string id)
        {
            if (_context.ProjectType == null)
            {
                return NotFound();
            }
            var ProjectType = await _context.ProjectType.FindAsync(id);
            if (ProjectType == null)
            {
                return NotFound();
            }

            _context.ProjectType.Remove(ProjectType);
            await _context.SaveChangesAsync();

            return ProjectType;
        }

        private bool ProjectTypeExists(string id)
        {
            return (_context.ProjectType?.Any(e => e.ProjectTypeID == id)).GetValueOrDefault();
        }
    }
}
