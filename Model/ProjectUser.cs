using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

[PrimaryKey("ProjectID", "UserID")]
public partial class ProjectUser  :IEntityRecord
{
    /// <summary>
    /// Código
    /// </summary>
    [Key]
     public int ProjectID { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string UserID { get; set; } = null!;

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectUser")]
  
    public virtual Project? Project { get; set; } = null!;

    [ForeignKey("UserID")]
    [InverseProperty("ProjectUser")]
  
    public virtual User? User { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
