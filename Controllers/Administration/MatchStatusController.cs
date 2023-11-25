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
    public class MatchStatusController : ControllerBase
    {
        private readonly Entities _context;

        public MatchStatusController(Entities context)
        {
            _context = context;
        }

        // GET: api/MatchStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchStatus>>> GetMatchStatus()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var matchStatusTranslation = DataTranslationLib.GetTranslation<MatchStatus>(_context, languageID);

            var list = (
                from MatchStatus in _context.MatchStatus
                join matchStatus in matchStatusTranslation on MatchStatus.MatchStatusID equals matchStatus.ID
                select new
                {
                    MatchStatus.MatchStatusID,
                    MatchStatusName = matchStatus.Name,
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.MatchStatusID == key);
            }

            if (parameters["MatchStatusID"] != null)
            {
                string matchStatusID = parameters["MatchStatusID"];
                list = list.Where(l => l.MatchStatusID == matchStatusID);
            }

            if (parameters["MatchStatusName"] != null)
            {
                string matchStatusName = parameters["MatchStatusName"];
                list = list.Where(l => l.MatchStatusName.Contains(matchStatusName));
                list = list.OrderBy(l => l.MatchStatusName.IndexOf(matchStatusName));
            }

            var ret = await list.ToListAsync();
            return Ok(ret);
        }

        // GET: api/MatchStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchStatus>> GetMatchStatus(string id)
        {
            if (_context.MatchStatus == null)
            {
                return NotFound();
            }
            var MatchStatus = await _context.MatchStatus.FindAsync(id);

            if (MatchStatus == null)
            {
                return NotFound();
            }

            return MatchStatus;
        }

        // PUT: api/MatchStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatchStatus(string id, MatchStatus MatchStatus)
        {
            if (id != MatchStatus.MatchStatusID)
            {
                return BadRequest();
            }

            _context.Entry(MatchStatus).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(_context, MatchStatus.DataTranslation.ToList());

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchStatusExists(id))
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

        // POST: api/MatchStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MatchStatus>> PostMatchStatus(MatchStatus MatchStatus)
        {
            if (_context.MatchStatus == null)
            {
                return Problem("Entity set 'Entities.MatchStatus'  is null.");
            }
            _context.MatchStatus.Add(MatchStatus);
            Entities.ProcessChildrenUpdate(_context, MatchStatus.DataTranslation.ToList());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatchStatus", new { id = MatchStatus.MatchStatusID }, MatchStatus);
        }

        // DELETE: api/MatchStatus/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MatchStatus>> DeleteMatchStatus(string id)
        {
            if (_context.MatchStatus == null)
            {
                return NotFound();
            }
            var MatchStatus = await _context.MatchStatus.FindAsync(id);
            if (MatchStatus == null)
            {
                return NotFound();
            }

            _context.MatchStatus.Remove(MatchStatus);
            await _context.SaveChangesAsync();

            return MatchStatus;
        }

        private bool MatchStatusExists(string id)
        {
            return (_context.MatchStatus?.Any(e => e.MatchStatusID == id)).GetValueOrDefault();
        }
    }
}
