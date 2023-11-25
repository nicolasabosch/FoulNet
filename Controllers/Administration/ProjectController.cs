using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoulNet;
using FoulNet.Model;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Collections.Specialized;
using CabernetDBContext;


namespace FoulNet.Controllers.Administration
{
    [Route("api/Project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly Entities _context;

        public ProjectController(Entities context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Project>>> GetProject()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };
            var projectTypeTranslation = DataTranslationLib.GetTranslation<ProjectType>(_context, languageID);
            var projectStatusTranslation = DataTranslationLib.GetTranslation<ProjectStatus>(_context, languageID);
            var list = (
                from Project in _context.Project
                join projectType in projectTypeTranslation on Project.ProjectTypeID equals projectType.ID
                join projectStatus in projectStatusTranslation on Project.ProjectStatusID equals projectStatus.ID
                join Customer in _context.Customer on Project.CustomerID equals Customer.CustomerID
                from File in _context.File.Where(X => X.FileID == Project.FileID).DefaultIfEmpty()
                select new
                {
                    Project.ProjectID,
                    Project.ProjectName,
                    Project.ProjectDate,
                    Project.ProjectTypeID,
                    Project.ProjectStatusID,
                    ProjectTypeName = projectType.Name,
                    ProjectStatusName = projectStatus.Name,
                    Project.CustomerID,
                    Customer.CustomerName,
                    File.FileName,
                    PreviewFileID = File == null ? false : previewList.Contains(File.FileName.ToLower().Substring(File.FileName.ToLower().IndexOf("."))),
                }

            );
            if (parameters["key"] != null)
            {
                int key = int.Parse(parameters["key"]);
                list = list.Where(l => l.ProjectID == key);
            }

            if (parameters["ProjectID"] != null)
            {
                int projectID = int.Parse(parameters["ProjectID"]);
                list = list.Where(l => l.ProjectID == projectID);
            }

            if (parameters["ProjectName"] != null)
            {
                string projectName = parameters["ProjectName"];
                list = list.Where(l => l.ProjectName.Contains(projectName));
                list = list.OrderBy(l => l.ProjectName.IndexOf(projectName));
            }

            if (parameters["ProjectDateFrom"] != null)
            {
                DateTime projectDateFrom = DateTime.Parse(parameters["ProjectDateFrom"]);
                list = list.Where(l => l.ProjectDate >= projectDateFrom);
            }

            if (parameters["ProjectDateTo"] != null)
            {
                DateTime projectDateTo = DateTime.Parse(parameters["ProjectDateTo"]);
                list = list.Where(l => l.ProjectDate <= projectDateTo);
            }

            if (parameters["ProjectTypeID"] != null)
            {
                string projectTypeID = parameters["ProjectTypeID"];
                list = list.Where(l => l.ProjectTypeID == projectTypeID);
            }

            if (parameters["ProjectStatusID"] != null)
            {
                string projectStatusID = parameters["ProjectStatusID"];
                list = list.Where(l => l.ProjectStatusID == projectStatusID);
            }
            if (parameters["CustomerID"] != null)
            {
                string customerID = parameters["CustomerID"];
                list = list.Where(l => l.CustomerID == customerID);
            }

            var ret =await list.ToListAsync();
            return Ok(ret);
            

        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        
        public async Task<ActionResult> GetProject(int id)
        {
            var languageID = this.CurrentLanguageID();
            var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };

            var project = await (
                from Project in _context.Project
                join Customer in _context.Customer on Project.CustomerID equals Customer.CustomerID
                from File in _context.File.Where(X => X.FileID == Project.FileID).DefaultIfEmpty()
                where Project.ProjectID == id
                select new
                {
                    Project.ProjectID,
                    Project.ProjectName,
                    Project.ProjectDate,
                    Project.ProjectTypeID,
                    Project.CustomerID,
                    Project.ProjectStatusID,
                    Project.FileID,
                    Project.CreatedOn,
                    Project.CreatedBy,
                    Project.LastModifiedOn,
                    Project.LastModifiedBy,

                    Customer = new
                    {
                        Project.CustomerID,
                        Customer.CustomerName
                    },

                    File.FileName,
                    PreviewFileID = File == null ? false : previewList.Contains(File.FileName.ToLower().Substring(File.FileName.ToLower().IndexOf("."))),
         
                }

            ).FirstOrDefaultAsync();
            if (project == null)
            {
                return NotFound();
            }

            dynamic record = project.ToExpando();
            record.ProjectExpense = await (
                from ProjectExpense in _context.ProjectExpense
                join PEVendor in _context.Vendor on ProjectExpense.VendorID equals PEVendor.VendorID
                from ProjectExpenseFile in _context.File.Where(X => X.FileID == ProjectExpense.FileID).DefaultIfEmpty()

                where ProjectExpense.ProjectID == id
                select new
                {
                    ProjectExpense.ProjectExpenseID,
                    ProjectExpense.ExpenseDate,
                    ProjectExpense.VendorID,
                    ProjectExpense.ConceptID,
                    ProjectExpense.ProjectID,
                    ProjectExpense.Quantity,
                    ProjectExpense.Amount,
                    ProjectExpense.FileID,
                    ProjectExpenseFile.FileName,
                    ProjectExpense.LastModifiedOn,
                    PreviewFileID = ProjectExpenseFile == null ? false : previewList.Contains(ProjectExpenseFile.FileName.ToLower().Substring(ProjectExpenseFile.FileName.ToLower().IndexOf("."))),
                    EntityStatus = "U",
                    Vendor = new
                    {
                        ProjectExpense.VendorID,
                        PEVendor.VendorName,
                    } ,
                   
             
                }

            ).ToListAsync();
            record.ProjectFile = await (
                from ProjectFile in _context.ProjectFile
                join PFFile in _context.File on ProjectFile.FileID equals PFFile.FileID
                where ProjectFile.ProjectID == id
                select new
                {
                    ProjectFile.ProjectFileID,
                    ProjectFile.ProjectID,
                    ProjectFile.FileID,
                    ProjectFile.Remarks,
                    ProjectFile.LastModifiedOn,
                    PFFile.FileName,
                    PreviewFileID = PFFile == null ? false : previewList.Contains(PFFile.FileName.ToLower().Substring(PFFile.FileName.ToLower().IndexOf("."))),
                    EntityStatus = "U",
                }

            ).ToListAsync();
            record.ProjectTool = await (
                from ProjectTool in _context.ProjectTool
                where ProjectTool.ProjectID == id
                select new
                {
                    ProjectTool.ProjectID,
                    ProjectTool.ToolID,
                    ProjectTool.LastModifiedOn,
                    EntityStatus = "U",
                }

            ).ToListAsync();
            record.ProjectUser = await (
                from ProjectUser in _context.ProjectUser
                join User in _context.User on ProjectUser.UserID equals User.UserID
                where ProjectUser.ProjectID == id
                select new
                {
                    ProjectUser.ProjectID,
                    ProjectUser.UserID,
                    ProjectUser.LastModifiedOn,
                    User.UserName,
                    EntityStatus = "U",

                }

            ).ToListAsync();
            return Ok(record);
        }


        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            ModelState.Clear();
            Extensions.ClearReferences(project);
            TryValidateModel(project);
            if (id != project.ProjectID)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;
            
            try
            {
                //    await _context.SaveChangesAsync();
                 _context.UpdateRecord(project);
            }
            catch (Exception)
            {
                return Conflict();
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {

            ModelState.Clear();
            Extensions.ClearReferences(project);
            TryValidateModel(project);
            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.ProjectID }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (_context.Project == null)
            {
                return NotFound();
            }
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return (_context.Project?.Any(e => e.ProjectID == id)).GetValueOrDefault();
        }
    }
}
