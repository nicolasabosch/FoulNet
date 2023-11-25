using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class Team  :IEntityRecord
{
    /// <summary>
    /// Código
    /// </summary>
    [Key]
     public int TeamID { get; set; }

    /// <summary>
    /// Nombre
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string TeamName { get; set; } = null!;

    /// <summary>
    /// Código
    /// </summary>
     public int ZoneID { get; set; }

    [StringLength(36)]
    [Unicode(false)]
     public string? FileID { get; set; }

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [ForeignKey("FileID")]
    [InverseProperty("Team")]
  
    public virtual File? File { get; set; }

    [InverseProperty("AwayTeam")]
    public virtual ICollection<Match> MatchAwayTeam { get; } = new List<Match>();

    [InverseProperty("HomeTeam")]
    public virtual ICollection<Match> MatchHomeTeam { get; } = new List<Match>();

    [InverseProperty("Team")]
    public virtual ICollection<TeamPlayer> TeamPlayer { get; } = new List<TeamPlayer>();

    [ForeignKey("ZoneID")]
    [InverseProperty("Team")]
  
    public virtual Zone? Zone { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
