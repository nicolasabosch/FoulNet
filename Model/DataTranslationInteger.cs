using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

[PrimaryKey("ID", "FieldName", "LanguageID")]
public partial class DataTranslationInteger  :IEntityRecord
{
    [Key]
    public int ID { get; set; }

    [Key]
    [StringLength(100)]
    [Unicode(false)]
    public string FieldName { get; set; } = null!;

    [Key]
    [StringLength(2)]
    [Unicode(false)]
    public string LanguageID { get; set; } = null!;

    [StringLength(1000)]
    public string Translation { get; set; } = null!;

    public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? LastModifiedBy { get; set; }

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
