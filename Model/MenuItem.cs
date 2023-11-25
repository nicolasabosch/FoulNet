using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class MenuItem  :IEntityRecord
{
    /// <summary>
    /// ID
    /// </summary>
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string MenuItemID { get; set; } = null!;

    /// <summary>
    /// Nombre
    /// </summary>
    [StringLength(100)]
     public string MenuItemName { get; set; } = null!;

    /// <summary>
    /// Menu
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string MenuBarID { get; set; } = null!;

    /// <summary>
    /// Orden
    /// </summary>
     public short DisplayOrder { get; set; }

     public short GroupNumber { get; set; }

    /// <summary>
    /// RouteName
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string RouteName { get; set; } = null!;

    /// <summary>
    /// Es Página
    /// </summary>
     public bool IsPage { get; set; }

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [InverseProperty("MenuItem")]
    public virtual ICollection<RoleMenuItem> RoleMenuItem { get; } = new List<RoleMenuItem>();

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
