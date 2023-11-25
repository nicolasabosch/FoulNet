using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class File  :IEntityRecord
{
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string FileID { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
     public string FileName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
     public string FolderName { get; set; } = null!;

     public DateTimeOffset? FileDate { get; set; }

     public int? FileSize { get; set; }

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<Project> Project { get; } = new List<Project>();

    [InverseProperty("File")]
    public virtual ICollection<ProjectExpense> ProjectExpense { get; } = new List<ProjectExpense>();

    [InverseProperty("File")]
    public virtual ICollection<ProjectFile> ProjectFile { get; } = new List<ProjectFile>();

    [InverseProperty("File")]
    public virtual ICollection<Team> Team { get; } = new List<Team>();

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
