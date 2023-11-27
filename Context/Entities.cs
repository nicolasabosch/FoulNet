using System;
using System.Collections.Generic;
using CabernetDBContext;
using File = FoulNet.Model.File;
using FoulNet.Model;
using Microsoft.EntityFrameworkCore;

namespace FoulNet;

public partial class Entities : DbEntities
{
    public Entities(DbContextOptions<DbEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<ClientError> ClientError { get; set; }

    public virtual DbSet<Concept> Concept { get; set; }

    public virtual DbSet<Customer> Customer { get; set; }

    public virtual DbSet<DataTable> DataTable { get; set; }

    public virtual DbSet<File> File { get; set; }

    public virtual DbSet<Language> Language { get; set; }

    public virtual DbSet<Match> Match { get; set; }

    public virtual DbSet<MatchStatus> MatchStatus { get; set; }

    public virtual DbSet<MenuBar> MenuBar { get; set; }

    public virtual DbSet<MenuItem> MenuItem { get; set; }

    public virtual DbSet<Parameter> Parameter { get; set; }

    public virtual DbSet<Project> Project { get; set; }

    public virtual DbSet<ProjectExpense> ProjectExpense { get; set; }

    public virtual DbSet<ProjectFile> ProjectFile { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatus { get; set; }

    public virtual DbSet<ProjectTool> ProjectTool { get; set; }

    public virtual DbSet<ProjectType> ProjectType { get; set; }

    public virtual DbSet<ProjectUser> ProjectUser { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<RoleMenuItem> RoleMenuItem { get; set; }

    public virtual DbSet<State> State { get; set; }

    public virtual DbSet<Team> Team { get; set; }

    public virtual DbSet<TeamPlayer> TeamPlayer { get; set; }

    public virtual DbSet<TextTranslation> TextTranslation { get; set; }

    public virtual DbSet<Tool> Tool { get; set; }

    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<UserRole> UserRole { get; set; }

    public virtual DbSet<Vendor> Vendor { get; set; }

    public virtual DbSet<Zone> Zone { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Concept>(entity =>
        {
            entity.Property(e => e.ConceptID).HasComment("Código");
            entity.Property(e => e.ConceptName).HasComment("Nombre");
        });


        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerID).HasComment("Código");
            entity.Property(e => e.Address).HasComment("Dirección");
            entity.Property(e => e.CustomerName).HasComment("Nombre");
            entity.Property(e => e.DistrictName).HasComment("Localidad");
            entity.Property(e => e.StateID).HasComment("Provinicia");
            entity.Property(e => e.VatCode).HasComment("IVA");
            entity.Property(e => e.ZipCode).HasComment("Código Postal");

            entity.HasOne(d => d.State).WithMany(p => p.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_State");
        });


        modelBuilder.Entity<DataTable>(entity =>
        {
            entity.Property(e => e.DataTableID).HasComment("Código");
            entity.Property(e => e.DataTableName).HasComment("Nombre");
        });


        modelBuilder.Entity<Language>(entity =>
        {
            entity.Property(e => e.LanguageID)
                .IsFixedLength()
                .HasComment("Código");
            entity.Property(e => e.LanguageName).HasComment("Nombre");
        });


        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(e => e.MatchID).HasComment("Código");
            entity.Property(e => e.AwayGoals).HasComment("Goles Visitante");
            entity.Property(e => e.AwayTeamID).HasComment("Equipo Visitante");
            entity.Property(e => e.HomeGoals).HasComment("Goles Local");
            entity.Property(e => e.HomeTeamID).HasComment("Equipo Local");
            entity.Property(e => e.MatchDate).HasComment("Fecha");
            entity.Property(e => e.MatchStatusID).HasComment("Código");
            entity.Property(e => e.UserID).HasComment("User Id");
            entity.Property(e => e.ZoneID).HasComment("Zona");

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Team1");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Team");

            entity.HasOne(d => d.MatchStatus).WithMany(p => p.Match)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_MatchStatus");

            entity.HasOne(d => d.User).WithMany(p => p.Match).HasConstraintName("FK_Match_User");

            entity.HasOne(d => d.Zone).WithMany(p => p.Match)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Zone");
        });


        modelBuilder.Entity<MatchStatus>(entity =>
        {
            entity.Property(e => e.MatchStatusID).HasComment("Código");
            entity.Property(e => e.MatchStatusName).HasComment("Nombre");
        });


        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.Property(e => e.MenuItemID).HasComment("ID");
            entity.Property(e => e.DisplayOrder).HasComment("Orden");
            entity.Property(e => e.IsPage).HasComment("Es Página");
            entity.Property(e => e.MenuBarID).HasComment("Menu");
            entity.Property(e => e.MenuItemName).HasComment("Nombre");
            entity.Property(e => e.RouteName).HasComment("RouteName");
        });


        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.ProjectID).HasComment("Código");
            entity.Property(e => e.CustomerID).HasComment("Cliente");
            entity.Property(e => e.FileID).HasComment("Imagen");
            entity.Property(e => e.ProjectDate).HasComment("Fecha");
            entity.Property(e => e.ProjectName).HasComment("Nombre");
            entity.Property(e => e.ProjectStatusID).HasComment("Activo");
            entity.Property(e => e.ProjectTypeID).HasComment("Tipo");

            entity.HasOne(d => d.Customer).WithMany(p => p.Project)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_Customer");

            entity.HasOne(d => d.File).WithMany(p => p.Project).HasConstraintName("FK_Project_File");

            entity.HasOne(d => d.ProjectStatus).WithMany(p => p.Project)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_ProjectStatus");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.Project)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_ProjectType");
        });


        modelBuilder.Entity<ProjectExpense>(entity =>
        {
            entity.Property(e => e.Amount).HasComment("Monto");
            entity.Property(e => e.ConceptID).HasComment("Código");
            entity.Property(e => e.ExpenseDate).HasComment("Fecha");
            entity.Property(e => e.FileID).HasComment("Imagen");
            entity.Property(e => e.ProjectID).HasComment("Código");
            entity.Property(e => e.Quantity).HasComment("Cantidad");
            entity.Property(e => e.VendorID).HasComment("Código");

            entity.HasOne(d => d.Concept).WithMany(p => p.ProjectExpense)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectExpense_Concept");

            entity.HasOne(d => d.File).WithMany(p => p.ProjectExpense).HasConstraintName("FK_ProjectExpense_File");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectExpense).HasConstraintName("FK_ProjectExpense_Project");

            entity.HasOne(d => d.Vendor).WithMany(p => p.ProjectExpense)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectExpense_Vendor");
        });


        modelBuilder.Entity<ProjectFile>(entity =>
        {
            entity.Property(e => e.ProjectID).HasComment("Código");

            entity.HasOne(d => d.File).WithMany(p => p.ProjectFile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectFile_File");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectFile).HasConstraintName("FK_ProjectFile_Project");
        });


        modelBuilder.Entity<ProjectTool>(entity =>
        {
            entity.Property(e => e.ProjectID).HasComment("Código");
            entity.Property(e => e.ToolID).HasComment("Código");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTool).HasConstraintName("FK_ProjectTool_Project");

            entity.HasOne(d => d.Tool).WithMany(p => p.ProjectTool)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectTool_Tool");
        });


        modelBuilder.Entity<ProjectType>(entity =>
        {
            entity.Property(e => e.ProjectTypeID).HasComment("Código");
            entity.Property(e => e.ProjectTypeName).HasComment("Nombre");
        });


        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.Property(e => e.ProjectID).HasComment("Código");
            entity.Property(e => e.UserID).HasComment("User Id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectUser).HasConstraintName("FK_ProjectUser_Project");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectUser_User");
        });


        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleID).HasComment("ID");
            entity.Property(e => e.RoleName).HasComment("Rol");
        });


        modelBuilder.Entity<RoleMenuItem>(entity =>
        {
            entity.HasOne(d => d.MenuItem).WithMany(p => p.RoleMenuItem).HasConstraintName("FK_RoleMenuItem_MenuItem");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenuItem).HasConstraintName("FK_RoleMenuItem_Role");
        });


        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.TeamID).HasComment("Código");
            entity.Property(e => e.TeamName).HasComment("Nombre");
            entity.Property(e => e.ZoneID).HasComment("Código");

            entity.HasOne(d => d.File).WithMany(p => p.Team).HasConstraintName("FK_Team_File");

            entity.HasOne(d => d.Zone).WithMany(p => p.Team)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_Zone");
        });


        modelBuilder.Entity<TeamPlayer>(entity =>
        {
            entity.Property(e => e.PlayerName).HasComment("Nombre Jugador");
            entity.Property(e => e.PlayerNumber).HasComment("Número");
            entity.Property(e => e.TeamID).HasComment("Código");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeamPlayer_Team");
        });


        modelBuilder.Entity<TextTranslation>(entity =>
        {
            entity.Property(e => e.LanguageID).IsFixedLength();
        });


        modelBuilder.Entity<Tool>(entity =>
        {
            entity.Property(e => e.ToolID).HasComment("Código");
            entity.Property(e => e.ToolName).HasComment("Nombre");
        });


        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserID).HasComment("User Id");
            entity.Property(e => e.Active).HasComment("Activo");
            entity.Property(e => e.Email).HasComment("E Mail");
            entity.Property(e => e.ForceChangePassword).HasComment("Forzar Cambio Contraseña");
            entity.Property(e => e.InvitedOn).HasComment("Fecha Invitación");
            entity.Property(e => e.LastLogon).HasComment("Último Login");
            entity.Property(e => e.LogonName).HasComment("Login");
            entity.Property(e => e.Password).HasComment("Password");
            entity.Property(e => e.ReceiveNotification).HasComment("Recibe Notificaciones");
            entity.Property(e => e.ResetPasswordID).HasComment("ResetPasswordID");
            entity.Property(e => e.UserName).HasComment("Nombre");
        });


        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserID, e.RoleID }).HasName("PK_ApplicationUserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRole).HasConstraintName("FK_ApplicationUserRole_ApplicationRole");

            entity.HasOne(d => d.User).WithMany(p => p.UserRole).HasConstraintName("FK_ApplicationUserRole_ApplicationUser");
        });


        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.Property(e => e.VendorID).HasComment("Código");
            entity.Property(e => e.VendorName).HasComment("Nombre");
        });


        modelBuilder.Entity<Zone>(entity =>
        {
            entity.Property(e => e.ZoneID).HasComment("Código");
            entity.Property(e => e.ZoneName).HasComment("Nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
