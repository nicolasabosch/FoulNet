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
    public class ProjectStatusController : ControllerBase
    {
        private readonly Entities _context;

        public ProjectStatusController(Entities context)
        {
            _context = context;
        }

        // GET: api/ProjectStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectStatus>>> GetProjectStatus()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var projectStatusTranslation = DataTranslationLib.GetTranslation<ProjectStatus>(_context, languageID);

            var list = (
                from ProjectStatus in _context.ProjectStatus
                join projectStatus in projectStatusTranslation on ProjectStatus.ProjectStatusID equals projectStatus.ID
                select new
                {
                    ProjectStatus.ProjectStatusID,
                    ProjectStatusName = projectStatus.Name,
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.ProjectStatusID == key);
            }

            if (parameters["ProjectStatusID"] != null)
            {
                string projectStatusID = parameters["ProjectStatusID"];
                list = list.Where(l => l.ProjectStatusID == projectStatusID);
            }

            if (parameters["ProjectStatusName"] != null)
            {
                string projectStatusName = parameters["ProjectStatusName"];
                list = list.Where(l => l.ProjectStatusName.Contains(projectStatusName));
                list = list.OrderBy(l => l.ProjectStatusName.IndexOf(projectStatusName));
            }

            var ret = await list.ToListAsync();
            return Ok(ret);
        }

        // GET: api/ProjectStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectStatus>> GetProjectStatus(string id)
        {
            if (_context.ProjectStatus == null)
            {
                return NotFound();
            }
            var ProjectStatus = await _context.ProjectStatus.FindAsync(id);

            if (ProjectStatus == null)
            {
                return NotFound();
            }

            return ProjectStatus;
        }

        // PUT: api/ProjectStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectStatus(string id, ProjectStatus ProjectStatus)
        {
            if (id != ProjectStatus.ProjectStatusID)
            {
                return BadRequest();
            }

            _context.Entry(ProjectStatus).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(_context, ProjectStatus.DataTranslation.ToList());

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectStatusExists(id))
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

        // POST: api/ProjectStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectStatus>> PostProjectStatus(ProjectStatus ProjectStatus)
        {
            if (_context.ProjectStatus == null)
            {
                return Problem("Entity set 'Entities.ProjectStatus'  is null.");
            }
            _context.ProjectStatus.Add(ProjectStatus);
            Entities.ProcessChildrenUpdate(_context, ProjectStatus.DataTranslation.ToList());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectStatus", new { id = ProjectStatus.ProjectStatusID }, ProjectStatus);
        }

        // DELETE: api/ProjectStatus/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProjectStatus>> DeleteProjectStatus(string id)
        {
            if (_context.ProjectStatus == null)
            {
                return NotFound();
            }
            var ProjectStatus = await _context.ProjectStatus.FindAsync(id);
            if (ProjectStatus == null)
            {
                return NotFound();
            }

            _context.ProjectStatus.Remove(ProjectStatus);
            await _context.SaveChangesAsync();

            return ProjectStatus;
        }

        private bool ProjectStatusExists(string id)
        {
            return (_context.ProjectStatus?.Any(e => e.ProjectStatusID == id)).GetValueOrDefault();
        }
    }
}
