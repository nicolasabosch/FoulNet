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

[Route("api/Match")]
[ApiController]
public class MatchController : ControllerBase
{
    private readonly Entities _context;
    private readonly ToolsLib _toolsLib;
    public MatchController(Entities context, ToolsLib toolsLib)
    {
        _toolsLib = toolsLib;
        _context = context;
    }

    // GET api/Match
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {

        NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
        var languageID = this.CurrentLanguageID();
        var matchTranslation = DataTranslationLib.GetTranslation<Match>(_context, languageID);


        var list = (
                 from Match in _context.Match
             join MatchStatus in _context.MatchStatus on Match.MatchStatusID equals MatchStatus.MatchStatusID
             join Zone in _context.Zone on Match.ZoneID equals Zone.ZoneID
             join Home in _context.Team on Match.HomeTeamID equals Home.TeamID
             join Away in _context.Team on Match.AwayTeamID equals Away.TeamID
             select new
             {
                 Match.MatchID,
                 Match.MatchDate,
                 Match.ZoneID,
                 Match.MatchStatusID,
                 MatchStatus.MatchStatusName,
                 Zone.ZoneName,
                 Match.HomeTeamID,
                 HomeTeamName = Home.TeamName,
                 AwayTeamName = Away.TeamName,
                 Match.AwayTeamID,
                 Match.HomeGoals,
                 Match.AwayGoals



             }

         );


        if (parameters["key"] != null)
        {
            int key = int.Parse(parameters["key"]);
            list = list.Where(l => l.MatchID == key);
        }

        if (parameters["MatchID"] != null)
        {
            int matchID = int.Parse(parameters["MatchID"]);
            list = list.Where(l => l.MatchID == matchID);
        }

        if (parameters["MatchDateFrom"] != null)
        {
            DateTime MatchDateFrom = DateTime.Parse(parameters["MatchDateFrom"]);
            list = list.Where(l => l.MatchDate >= MatchDateFrom);
        }

        if (parameters["MatchDateTo"] != null)
        {
            DateTime MatchDateTo = DateTime.Parse(parameters["MatchDateTo"]);
            list = list.Where(l => l.MatchDate <= MatchDateTo);
        }

        if (parameters["ZoneID"] != null)
        {
            int zoneID = int.Parse(parameters["ZoneID"]);
            list = list.Where(l => l.ZoneID == zoneID);
        }

        if (parameters["MatchStatusID"] != null)
        {
            string MatchStatusID = parameters["MatchStatusID"];
            list = list.Where(l => l.MatchStatusID == MatchStatusID);
        }

        var ret = await list.ToListAsync();
        return Ok(ret);
    }

    // GET: api/Match/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetMatch(int id)
    {
        if (_context.Match == null)
        {
            return NotFound();
        }
        var match = await (
     from Match in _context.Match
     join MatchStatus in _context.MatchStatus on Match.MatchStatusID equals MatchStatus.MatchStatusID
     join Zone in _context.Zone on Match.ZoneID equals Zone.ZoneID
     join Home in _context.Team on Match.HomeTeamID equals Home.TeamID
     join Away in _context.Team on Match.AwayTeamID equals Away.TeamID
     where Match.MatchID == id
     select new
     {
         Match.MatchID,
         Match.MatchDate,
         Match.ZoneID,
         Match.MatchStatusID,
         MatchStatus.MatchStatusName,
         Zone.ZoneName,
         Match.HomeTeamID,
         HomeTeamName = Home.TeamName,
         AwayTeamName = Away.TeamName,
         Match.AwayTeamID,
         HomeGoals = Match.HomeGoals??0,
         AwayGoals = Match.AwayGoals??0,
         //Match.LastModifiedOn,
         localFileID = Home.FileID,
         visitanteFileID = Away.FileID,
         HomeTeam = new { Home.TeamID, Home.TeamName },
         AwayTeam = new { Away.TeamID, Away.TeamName },
         local= Home.TeamName,
         visitante = Away.TeamName


     }).FirstAsync();

        dynamic record = match.ToExpando();
        record.localJugadores = (from TeamPlayer in _context.TeamPlayer
                                 where TeamPlayer.TeamID == match.HomeTeamID
                                 select new
                                 {
                                     Nombre = TeamPlayer.PlayerName,
                                     Camiseta = TeamPlayer.PlayerNumber,
                                     Puntos = 0,
                                     Faltas = 0,
                                 }
                                 ).ToList();

        record.visitanteJugadores = (from TeamPlayer in _context.TeamPlayer
                                 where TeamPlayer.TeamID == match.AwayTeamID
                                 select new
                                 {
                                     Nombre = TeamPlayer.PlayerName,
                                     Camiseta = TeamPlayer.PlayerNumber,
                                     Puntos = 0,
                                     Faltas = 0,
                                 }
                                 ).ToList();

        return record;
    }

    // PUT: api/Match/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMatch(int id, Match Match)
    {
        if (id != Match.MatchID)
        {
            return BadRequest();
        }

        _context.Entry(Match).State = EntityState.Modified;
        Entities.ProcessChildrenUpdate(_context, Match.DataTranslation.ToList());

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MatchExists(id))
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

    // POST: api/Match
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Match>> PostMatch(Match Match)
    {
        ModelState.Clear();
        Extensions.ClearReferences(Match);

        if (_context.Match == null)
        {
            return Problem("Entity set 'Entities.Match'  is null.");
        }
        _context.Match.Add(Match);
        //Entities.ProcessChildrenUpdate(_context, Match.DataTranslation.ToList());
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMatch", new { id = Match.MatchID }, Match);
    }

    // DELETE: api/Match/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Match>> DeleteMatch(int id)
    {
        if (_context.Match == null)
        {
            return NotFound();
        }
        var Match = await _context.Match.FindAsync(id);
        if (Match == null)
        {
            return NotFound();
        }

        _context.Match.Remove(Match);
        await _context.SaveChangesAsync();

        return Match;
    }

    private bool MatchExists(int id)
    {
        return (_context.Match?.Any(e => e.MatchID == id)).GetValueOrDefault();
    }
}