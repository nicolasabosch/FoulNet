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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FoulNet.Model;

[Route("api/Team")]
[ApiController]
public class TeamController : ControllerBase
{
    private readonly Entities _context;
    private readonly ToolsLib _toolsLib;
    public TeamController(Entities context, ToolsLib toolsLib)
    {
        _context = context;
        _toolsLib = toolsLib;
    }

    // GET api/Team
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetAll()
    {

        NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
        var languageID = this.CurrentLanguageID();
        var teamTranslation = DataTranslationLib.GetTranslation<Team>(_context, languageID);
        var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };

        var list = (
             from Team in _context.Team
             join Zone in _context.Zone on Team.ZoneID equals Zone.ZoneID
             from File in _context.File.Where(X => X.FileID == Team.FileID).DefaultIfEmpty()
             select new
             {
                 Team.TeamID,
                 Team.TeamName,
                 Team.ZoneID,
                 Zone.ZoneName,
                 Team.FileID,
                 File.FileName,
                 PreviewFileID = File == null ? false : previewList.Contains(File.FileName.ToLower().Substring(File.FileName.ToLower().IndexOf("."))),
             }

         );


        if (parameters["key"] != null)
        {
            int key = int.Parse(parameters["key"]);
            list = list.Where(l => l.TeamID == key);
        }

        if (parameters["TeamID"] != null)
        {
            int teamID = int.Parse(parameters["TeamID"]);
            list = list.Where(l => l.TeamID == teamID);
        }


        if (parameters["ZoneID"] != null)
        {
            int zoneID = int.Parse(parameters["ZoneID"]);
            list = list.Where(l => l.ZoneID == zoneID);
        }


        if (parameters["TeamName"] != null)
        {
            string teamName = parameters["TeamName"];
            list = list.Where(l => l.TeamName.Contains(teamName));
            list = list.OrderBy(l => l.TeamName.IndexOf(teamName));
        }

        var ret = await list.ToListAsync();
        return Ok(ret);
    }

    // GET: api/Team/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetTeam(int id)
    {
        var previewList = new[] { ".jpeg", ".jpg", ".png", ".gif", ".bmp" };
        if (_context.Team == null)
        {
            return NotFound();
        }
        var team = await (from Team in _context.Team

                          from File in _context.File.Where(X => X.FileID == Team.FileID).DefaultIfEmpty()
                          where Team.TeamID == id
                          select new
                          {
                              Team.TeamID,
                              Team.TeamName,
                              Team.ZoneID,
                              Team.FileID,
                              File.FileName,
                              Team.LastModifiedOn,
                              PreviewFileID = File == null ? false : previewList.Contains(File.FileName.ToLower().Substring(File.FileName.ToLower().IndexOf("."))),

                          }).FirstAsync();

        dynamic record = team.ToExpando();
        record.TeamPlayer = await (from TeamPlayer in _context.TeamPlayer
                                   where TeamPlayer.TeamID == id
                                   select new
                                   {
                                       TeamPlayer.TeamPlayerID,
                                       TeamPlayer.TeamID,
                                       TeamPlayer.PlayerName,
                                       TeamPlayer.PlayerNumber,
                                       EntityStatus = "U",

                                   }).ToListAsync();
        return record;
    }

    // PUT: api/Team/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeam(int id, Team Team)
    {
        if (id != Team.TeamID)
        {
            return BadRequest();
        }

        _context.Entry(Team).State = EntityState.Modified;
        Entities.ProcessChildrenUpdate(_context, Team.TeamPlayer.ToList());

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeamExists(id))
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

    // POST: api/Team
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Team>> PostTeam(Team team)
    {
        if (team == null)
        {
            return Problem("Entity set 'Entities.Team'  is null.");
        }
        _context.Team.Add(team);
        //Entities.ProcessChildrenUpdate(_context, Team.DataTranslation.ToList());
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTeam", new { id = team.TeamID }, team);
    }

    // DELETE: api/Team/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Team>> DeleteTeam(int id)
    {
        if (_context.Team == null)
        {
            return NotFound();
        }
        var Team = await _context.Team.FindAsync(id);
        if (Team == null)
        {
            return NotFound();
        }

        _context.Team.Remove(Team);
        await _context.SaveChangesAsync();

        return Team;
    }

    private bool TeamExists(int id)
    {
        return (_context.Team?.Any(e => e.TeamID == id)).GetValueOrDefault();
    }
}