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
    public class ToolController : ControllerBase
    {
        private readonly Entities _context;

        public ToolController(Entities context)
        {
            _context = context;
        }

        // GET: api/Tool
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tool>>> GetTool()
        {
            var languageID = this.CurrentLanguageID();
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var toolTranslation = DataTranslationLib.GetTranslation<Tool>(_context, languageID);

            var list = (
                from Tool in _context.Tool
                join tool in toolTranslation on Tool.ToolID equals tool.ID
                select new
                {
                    Tool.ToolID,
                    ToolName = tool.Name,
                }

            );

            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.ToolID == key);
            }

            if (parameters["ToolID"] != null)
            {
                string toolID = parameters["ToolID"];
                list = list.Where(l => l.ToolID == toolID);
            }

            if (parameters["ToolName"] != null)
            {
                string toolName = parameters["ToolName"];
                list = list.Where(l => l.ToolName.Contains(toolName));
                list = list.OrderBy(l => l.ToolName.IndexOf(toolName));
            }

            var ret = await list.ToListAsync();
            return Ok(ret);
        }

        // GET: api/Tool/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tool>> GetTool(string id)
        {
            if (_context.Tool == null)
            {
                return NotFound();
            }
            var Tool = await _context.Tool.FindAsync(id);

            if (Tool == null)
            {
                return NotFound();
            }

            return Tool;
        }

        // PUT: api/Tool/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTool(string id, Tool Tool)
        {
            if (id != Tool.ToolID)
            {
                return BadRequest();
            }

            _context.Entry(Tool).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(_context, Tool.DataTranslation.ToList());

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToolExists(id))
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

        // POST: api/Tool
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tool>> PostTool(Tool Tool)
        {
            if (_context.Tool == null)
            {
                return Problem("Entity set 'Entities.Tool'  is null.");
            }
            _context.Tool.Add(Tool);
            Entities.ProcessChildrenUpdate(_context, Tool.DataTranslation.ToList());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTool", new { id = Tool.ToolID }, Tool);
        }

        // DELETE: api/Tool/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tool>> DeleteTool(string id)
        {
            if (_context.Tool == null)
            {
                return NotFound();
            }
            var Tool = await _context.Tool.FindAsync(id);
            if (Tool == null)
            {
                return NotFound();
            }

            _context.Tool.Remove(Tool);
            await _context.SaveChangesAsync();

            return Tool;
        }

        private bool ToolExists(string id)
        {
            return (_context.Tool?.Any(e => e.ToolID == id)).GetValueOrDefault();
        }
    }
}
