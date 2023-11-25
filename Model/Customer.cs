using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class Customer  :IEntityRecord
{
    /// <summary>
    /// Código
    /// </summary>
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string CustomerID { get; set; } = null!;

    /// <summary>
    /// Nombre
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string CustomerName { get; set; } = null!;

    /// <summary>
    /// Dirección
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string? Address { get; set; }

    /// <summary>
    /// Código Postal
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string? ZipCode { get; set; }

    /// <summary>
    /// Localidad
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string? DistrictName { get; set; }

    /// <summary>
    /// IVA
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string? VatCode { get; set; }

    /// <summary>
    /// Provinicia
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string StateID { get; set; } = null!;

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Project> Project { get; } = new List<Project>();

    [ForeignKey("StateID")]
    [InverseProperty("Customer")]
  
    public virtual State? State { get; set; } = null!;

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
