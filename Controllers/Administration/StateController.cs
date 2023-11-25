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
    public class StateController : ControllerBase
    {
        private readonly Entities _context;

        public StateController(Entities context)
        {
            _context = context;
        }

        // GET: api/State
        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetState()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };
            var list = (
                from State in _context.State
                select new
                {
                    State.StateID,
                    State.StateName,
                }

            );
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.StateID == key);
            }

            if (parameters["StateID"] != null)
            {
                string stateID = parameters["StateID"];
                list = list.Where(l => l.StateID == stateID);
            }

            if (parameters["StateName"] != null)
            {
                string stateName = parameters["StateName"];
                list = list.Where(l => l.StateName.Contains(stateName));
                list = list.OrderBy(l => l.StateName.IndexOf(stateName));
            }

            var ret = await list.ToListAsync();
            return Ok(ret);
        }

        // GET: api/State/5
        [HttpGet("{id}")]
        public async Task<ActionResult<State>> GetState(string id)
        {
            if (_context.State == null)
            {
                return NotFound();
            }
            var State = await _context.State.FindAsync(id);

            if (State == null)
            {
                return NotFound();
            }

            return State;
        }

        // PUT: api/State/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutState(string id, State State)
        {
            if (id != State.StateID)
            {
                return BadRequest();
            }

            _context.Entry(State).State = EntityState.Modified;
            Entities.ProcessChildrenUpdate(_context, State.DataTranslation.ToList());

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(id))
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

        // POST: api/State
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<State>> PostState(State State)
        {
            if (_context.State == null)
            {
                return Problem("Entity set 'Entities.State'  is null.");
            }
            _context.State.Add(State);
            Entities.ProcessChildrenUpdate(_context, State.DataTranslation.ToList());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetState", new { id = State.StateID }, State);
        }

        // DELETE: api/State/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<State>> DeleteState(string id)
        {
            if (_context.State == null)
            {
                return NotFound();
            }
            var State = await _context.State.FindAsync(id);
            if (State == null)
            {
                return NotFound();
            }

            _context.State.Remove(State);
            await _context.SaveChangesAsync();

            return State;
        }

        private bool StateExists(string id)
        {
            return (_context.State?.Any(e => e.StateID == id)).GetValueOrDefault();
        }
    }
}
