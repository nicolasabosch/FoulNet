using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

[PrimaryKey("RoleID", "MenuItemID")]
public partial class RoleMenuItem  :IEntityRecord
{
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string RoleID { get; set; } = null!;

    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string MenuItemID { get; set; } = null!;

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [ForeignKey("MenuItemID")]
    [InverseProperty("RoleMenuItem")]
  
    public virtual MenuItem? MenuItem { get; set; } = null!;

    [ForeignKey("RoleID")]
    [InverseProperty("RoleMenuItem")]
  
    public virtual Role? Role { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
