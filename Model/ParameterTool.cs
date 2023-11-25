using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

[PrimaryKey("ParameterID", "ToolID")]
public partial class ParameterTool  :IEntityRecord
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string ParameterID { get; set; } = null!;

    /// <summary>
    /// Código
    /// </summary>
    [Key]
    [StringLength(36)]
    [Unicode(false)]
    public string ToolID { get; set; } = null!;

    public string? ParameterValue { get; set; }

    public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("ParameterID")]
    [InverseProperty("ParameterTool")]
    public virtual Parameter? Parameter { get; set; } = null!;

    [ForeignKey("ToolID")]
    [InverseProperty("ParameterTool")]
    public virtual Tool? Tool { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
