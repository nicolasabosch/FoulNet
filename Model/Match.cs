using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class Match  :IEntityRecord
{
    /// <summary>
    /// Código
    /// </summary>
    [Key]
     public int MatchID { get; set; }

    /// <summary>
    /// Fecha
    /// </summary>
    [Column(TypeName = "date")]
     public DateTime MatchDate { get; set; }

    /// <summary>
    /// Zona
    /// </summary>
     public int ZoneID { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string MatchStatusID { get; set; } = null!;

    /// <summary>
    /// Equipo Local
    /// </summary>
     public int HomeTeamID { get; set; }

    /// <summary>
    /// Equipo Visitante
    /// </summary>
     public int AwayTeamID { get; set; }

    /// <summary>
    /// Goles Local
    /// </summary>
     public int? HomeGoals { get; set; }

    /// <summary>
    /// Goles Visitante
    /// </summary>
     public int? AwayGoals { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string? UserID { get; set; }

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [ForeignKey("AwayTeamID")]
    [InverseProperty("MatchAwayTeam")]
  
    public virtual Team? AwayTeam { get; set; } = null!;

    [ForeignKey("HomeTeamID")]
    [InverseProperty("MatchHomeTeam")]
  
    public virtual Team? HomeTeam { get; set; } = null!;

    [ForeignKey("MatchStatusID")]
    [InverseProperty("Match")]
  
    public virtual MatchStatus? MatchStatus { get; set; } = null!;

    [ForeignKey("UserID")]
    [InverseProperty("Match")]
  
    public virtual User? User { get; set; }

    [ForeignKey("ZoneID")]
    [InverseProperty("Match")]
  
    public virtual Zone? Zone { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
