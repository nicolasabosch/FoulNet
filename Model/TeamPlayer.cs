using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class TeamPlayer  :IEntityRecord
{
    [Key]
     public int TeamPlayerID { get; set; }

    /// <summary>
    /// Código
    /// </summary>
     public int TeamID { get; set; }

    /// <summary>
    /// Nombre Jugador
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string PlayerName { get; set; } = null!;

    /// <summary>
    /// Número
    /// </summary>
     public int PlayerNumber { get; set; }

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [ForeignKey("TeamID")]
    [InverseProperty("TeamPlayer")]
  
    public virtual Team? Team { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
