using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class ProjectExpense  :IEntityRecord
{
    [Key]
     public int ProjectExpenseID { get; set; }

    /// <summary>
    /// Fecha
    /// </summary>
    [Column(TypeName = "date")]
     public DateTime? ExpenseDate { get; set; }

    /// <summary>
    /// Código
    /// </summary>
     public int VendorID { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string ConceptID { get; set; } = null!;

    /// <summary>
    /// Código
    /// </summary>
     public int ProjectID { get; set; }

    /// <summary>
    /// Cantidad
    /// </summary>
     public int Quantity { get; set; }

    /// <summary>
    /// Monto
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
     public decimal Amount { get; set; }

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

    [ForeignKey("ConceptID")]
    [InverseProperty("ProjectExpense")]
  
    public virtual Concept? Concept { get; set; } = null!;

    [ForeignKey("FileID")]
    [InverseProperty("ProjectExpense")]
  
    public virtual File? File { get; set; }

    [ForeignKey("ProjectID")]
    [InverseProperty("ProjectExpense")]
  
    public virtual Project? Project { get; set; } = null!;

    [ForeignKey("VendorID")]
    [InverseProperty("ProjectExpense")]
  
    public virtual Vendor? Vendor { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
