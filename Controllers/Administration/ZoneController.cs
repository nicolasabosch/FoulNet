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

[Route("api/Zone")]
[ApiController]
public class ZoneController : ControllerBase
{
    private readonly Entities _context;
    private readonly ToolsLib _toolsLib;
    public ZoneController(Entities context, ToolsLib toolsLib)
    {
        _context = context;
        _toolsLib = toolsLib;
    }

    // GET api/Zone
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Zone>>> GetAll()
    {

        NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
        var languageID = this.CurrentLanguageID();
        var zoneTranslation = DataTranslationLib.GetTranslation<Zone>(_context, languageID);


        var list = (
             from Zone in _context.Zone
             
             select new
             {
                 Zone.ZoneID,
                 Zone.ZoneName,
             }

         );


        if (parameters["key"] != null)
        {
            int key = int.Parse( parameters["key"]);
            list = list.Where(l => l.ZoneID == key);
        }

        if (parameters["ZoneID"] != null)
        {
            int zoneID = int.Parse(parameters["ZoneID"]);
            list = list.Where(l => l.ZoneID == zoneID);
        }

        if (parameters["ZoneName"] != null)
        {
            string zoneName = parameters["ZoneName"];
            list = list.Where(l => l.ZoneName.Contains(zoneName));
            list = list.OrderBy(l => l.ZoneName.IndexOf(zoneName));
        }

        var ret = await list.ToListAsync();
        return Ok(ret);
    }

    // GET: api/Zone/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Zone>> GetZone(int id)
    {
        if (_context.Zone == null)
        {
            return NotFound();
        }
        var Zone = await _context.Zone.FindAsync(id);

        if (Zone == null)
        {
            return NotFound();
        }

        return Zone;
    }

    // PUT: api/Zone/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutZone(int id, Zone Zone)
    {
        if (id != Zone.ZoneID)
        {
            return BadRequest();
        }

        _context.Entry(Zone).State = EntityState.Modified;
        Entities.ProcessChildrenUpdate(_context, Zone.DataTranslation.ToList());

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ZoneExists(id))
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

    // POST: api/Zone
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Zone>> PostZone(Zone Zone)
    {
        if (_context.Zone == null)
        {
            return Problem("Entity set 'Entities.Zone'  is null.");
        }
        _context.Zone.Add(Zone);
        //Entities.ProcessChildrenUpdate(_context, Zone.DataTranslation.ToList());
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetZone", new { id = Zone.ZoneID }, Zone);
    }

    // DELETE: api/Zone/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Zone>> DeleteZone(int id)
    {
        if (_context.Zone == null)
        {
            return NotFound();
        }
        var Zone = await _context.Zone.FindAsync(id);
        if (Zone == null)
        {
            return NotFound();
        }

        _context.Zone.Remove(Zone);
        await _context.SaveChangesAsync();

        return Zone;
    }

    private bool ZoneExists(int id)
    {
        return (_context.Zone?.Any(e => e.ZoneID == id)).GetValueOrDefault();
    }
}