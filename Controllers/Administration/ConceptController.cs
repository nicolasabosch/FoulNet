using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Collections.Specialized;


using CabernetDBContext;
using FoulNet.Library;
using Microsoft.AspNetCore.Authorization;

namespace FoulNet.Model;

[Route("api/Concept")]
[ApiController]
public class ConceptController : ControllerBase
{
    private readonly Entities _context;
    private readonly ToolsLib _toolsLib;
    public ConceptController(Entities context, ToolsLib toolsLib)
    {
        _context = context;
        _toolsLib = toolsLib;
    }

    // GET api/Concept
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Concept>>> GetAll()
    {

        NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
        var languageID = this.CurrentLanguageID();
        var conceptTranslation = DataTranslationLib.GetTranslation<Concept>(_context, languageID);


        var list = (
             from Concept in _context.Concept
             join concept in conceptTranslation on Concept.ConceptID equals concept.ID
             select new
             {
                 Concept.ConceptID,
                 Concept.Active,
                 ConceptName = concept.Name,
             }

         );

        
        if (parameters["key"] != null)
        {
            string key = parameters["key"];
            list = list.Where(l => l.ConceptID == key);
        }

        if (parameters["ConceptID"] != null)
        {
            string conceptID = parameters["ConceptID"];
            list = list.Where(l => l.ConceptID == conceptID);
        }

        if (parameters["ConceptName"] != null)
        {
            string conceptName = parameters["ConceptName"];
            list = list.Where(l => l.ConceptName.Contains(conceptName));
            list = list.OrderBy(l => l.ConceptName.IndexOf(conceptName));
        }

        var ret = await list.ToListAsync();
        return Ok(ret);
    }

    // GET: api/Concept/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Concept>> GetConcept(string id)
    {
        if (_context.Concept == null)
        {
            return NotFound();
        }
        var Concept = await _context.Concept.FindAsync(id);

        if (Concept == null)
        {
            return NotFound();
        }

        return Concept;
    }

    // PUT: api/Concept/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutConcept(string id, Concept Concept)
    {
        if (id != Concept.ConceptID)
        {
            return BadRequest();
        }

        _context.Entry(Concept).State = EntityState.Modified;
        Entities.ProcessChildrenUpdate(_context, Concept.DataTranslation.ToList());

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ConceptExists(id))
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

    // POST: api/Concept
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Concept>> PostConcept(Concept Concept)
    {
        if (_context.Concept == null)
        {
            return Problem("Entity set 'Entities.Concept'  is null.");
        }
        _context.Concept.Add(Concept);
        //Entities.ProcessChildrenUpdate(_context, Concept.DataTranslation.ToList());
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetConcept", new { id = Concept.ConceptID }, Concept);
    }

    // DELETE: api/Concept/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Concept>> DeleteConcept(string id)
    {
        if (_context.Concept == null)
        {
            return NotFound();
        }
        var Concept = await _context.Concept.FindAsync(id);
        if (Concept == null)
        {
            return NotFound();
        }

        _context.Concept.Remove(Concept);
        await _context.SaveChangesAsync();

        return Concept;
    }

    private bool ConceptExists(string id)
    {
        return (_context.Concept?.Any(e => e.ConceptID == id)).GetValueOrDefault();
    }
}