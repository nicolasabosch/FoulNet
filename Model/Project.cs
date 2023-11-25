using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class Project  :IEntityRecord
{
    /// <summary>
    /// Código
    /// </summary>
    [Key]
     public int ProjectID { get; set; }

    /// <summary>
    /// Nombre
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string ProjectName { get; set; } = null!;

    /// <summary>
    /// Fecha
    /// </summary>
    [Column(TypeName = "date")]
     public DateTime ProjectDate { get; set; }

    /// <summary>
    /// Tipo
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string ProjectTypeID { get; set; } = null!;

    /// <summary>
    /// Activo
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string ProjectStatusID { get; set; } = null!;

    /// <summary>
    /// Cliente
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string CustomerID { get; set; } = null!;

    /// <summary>
    /// Imagen
    /// </summary>
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

    [ForeignKey("CustomerID")]
    [InverseProperty("Project")]
  
    public virtual Customer? Customer { get; set; } = null!;

    [ForeignKey("FileID")]
    [InverseProperty("Project")]
  
    public virtual File? File { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<ProjectExpense> ProjectExpense { get; } = new List<ProjectExpense>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectFile> ProjectFile { get; } = new List<ProjectFile>();

    [ForeignKey("ProjectStatusID")]
    [InverseProperty("Project")]
  
    public virtual ProjectStatus? ProjectStatus { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<ProjectTool> ProjectTool { get; } = new List<ProjectTool>();

    [ForeignKey("ProjectTypeID")]
    [InverseProperty("Project")]
  
    public virtual ProjectType? ProjectType { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<ProjectUser> ProjectUser { get; } = new List<ProjectUser>();

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
