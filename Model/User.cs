using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabernetDBContext;
using Microsoft.EntityFrameworkCore;

namespace FoulNet.Model;

public partial class User  :IEntityRecord
{
    /// <summary>
    /// User Id
    /// </summary>
    [Key]
    [StringLength(36)]
    [Unicode(false)]
     public string UserID { get; set; } = null!;

    /// <summary>
    /// Login
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
     public string LogonName { get; set; } = null!;

    /// <summary>
    /// Password
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
     public string Password { get; set; } = null!;

    /// <summary>
    /// Nombre
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
     public string UserName { get; set; } = null!;

    /// <summary>
    /// Activo
    /// </summary>
     public bool Active { get; set; }

    /// <summary>
    /// Forzar Cambio Contraseña
    /// </summary>
     public bool? ForceChangePassword { get; set; }

    /// <summary>
    /// E Mail
    /// </summary>
    [StringLength(200)]
    [Unicode(false)]
     public string Email { get; set; } = null!;

    /// <summary>
    /// Último Login
    /// </summary>
     public DateTimeOffset? LastLogon { get; set; }

    /// <summary>
    /// Recibe Notificaciones
    /// </summary>
     public bool ReceiveNotification { get; set; }

    /// <summary>
    /// ResetPasswordID
    /// </summary>
    [StringLength(36)]
    [Unicode(false)]
     public string? ResetPasswordID { get; set; }

    /// <summary>
    /// Fecha Invitación
    /// </summary>
     public DateTimeOffset? InvitedOn { get; set; }

     public DateTimeOffset? CreatedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? CreatedBy { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? LastModifiedOn { get; set; }

    [StringLength(200)]
    [Unicode(false)]
     public string? LastModifiedBy { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Match> Match { get; } = new List<Match>();

    [InverseProperty("User")]
    public virtual ICollection<ProjectUser> ProjectUser { get; } = new List<ProjectUser>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRole { get; } = new List<UserRole>();

    [NotMapped]
	public string? EntityStatus { get; set; }

	[NotMapped]
	public Dictionary<string, object>? OriginalValues { get; set; }

    [NotMapped]
	public virtual ICollection<DataTranslation>? DataTranslation { get; set; }


}
