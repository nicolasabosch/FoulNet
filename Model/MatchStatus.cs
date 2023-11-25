using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class MatchStatus  :IEntityRecord
{
    /// <summary>
    /// Código
    /// </summary>
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string MatchStatusID { get; set; } = null!;

    /// <summary>
    /// Nombre
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string MatchStatusName { get; set; } = null!;

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [InverseProperty("MatchStatus")]
    public virtual ICollection<Match> Match { get; } = new List<Match>();

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
