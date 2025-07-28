using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

public partial class UserDbContext : DbContext
{
    public UserDbContext()
    {
    }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserEmail> UserEmails { get; set; }

    public virtual DbSet<UserPhone> UserPhones { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pk");

            entity.ToTable("roles", "auth");

            entity.HasIndex(e => e.NormalizedName, "roles_normalized_name_uindex").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdat");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Issystem)
                .HasDefaultValue(false)
                .HasColumnName("issystem");
            entity.Property(e => e.Name)
                .HasMaxLength(24)
                .HasColumnName("name");
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(24)
                .HasColumnName("normalized_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users", "auth");

            entity.HasIndex(e => e.NormalizedUserName, "users_normalized_user_name_index")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.NormalizedUserName, "users_normalized_user_name_uindex").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AccessFailedCount)
                .HasDefaultValue(0)
                .HasColumnName("access_failed_count");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(e => e.NormalizedUserName)
                .HasMaxLength(36)
                .HasColumnName("normalized_user_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.TwoFactorEnabled)
                .HasDefaultValue(false)
                .HasColumnName("two_factor_enabled");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(36)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<UserEmail>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.NormalizedEmail }).HasName("user_emails_pk");

            entity.ToTable("user_emails", "auth");

            entity.HasIndex(e => e.NormalizedEmail, "user_emails_normalized_email_index")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.NormalizedEmail, "user_emails_normalized_email_uindex").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.IsPrimary }, "user_emails_user_id_is_primary_uindex")
                .IsUnique()
                .HasFilter("(is_primary = true)");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(255)
                .HasColumnName("normalized_email");
            entity.Property(e => e.Confirmed)
                .HasDefaultValue(false)
                .HasColumnName("confirmed");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailType)
                .HasMaxLength(50)
                .HasColumnName("email_type");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValue(false)
                .HasColumnName("is_primary");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithMany(p => p.UserEmails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_emails_users_id_fk");
        });

        modelBuilder.Entity<UserPhone>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.NormalizedPhone }).HasName("user_phones_pk");

            entity.ToTable("user_phones", "auth");

            entity.HasIndex(e => e.NormalizedPhone, "user_phones_normalized_phone_uindex").IsUnique();

            entity.HasIndex(e => e.UserId, "user_phones_user_id_uindex")
                .IsUnique()
                .HasFilter("(is_primary = true)");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.NormalizedPhone)
                .HasMaxLength(32)
                .HasColumnName("normalized_phone");
            entity.Property(e => e.Confirmed)
                .HasDefaultValue(false)
                .HasColumnName("confirmed");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValue(false)
                .HasColumnName("is_primary");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(32)
                .HasColumnName("phone_number");
            entity.Property(e => e.PhoneType)
                .HasMaxLength(32)
                .HasColumnName("phone_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithOne(p => p.UserPhone)
                .HasForeignKey<UserPhone>(d => d.UserId)
                .HasConstraintName("user_phones_user_id_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("user_roles_pk");

            entity.ToTable("user_roles", "auth");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("assigned_at");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_roles_roles_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_roles_users_id_fk");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_tokens_pk");

            entity.ToTable("user_tokens", "auth");

            entity.HasIndex(e => e.ExpiresAt, "user_tokens_expires_at_index").HasFilter("((revoked = false) AND (expires_at IS NOT NULL))");

            entity.HasIndex(e => e.Permissions, "user_tokens_permissions_index").HasMethod("gin");

            entity.HasIndex(e => e.TokenHash, "user_tokens_token_hash_uindex").IsUnique();

            entity.HasIndex(e => e.UserId, "user_tokens_user_id_index");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(255)
                .HasColumnName("device_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("issued_at");
            entity.Property(e => e.Permissions).HasColumnName("permissions");
            entity.Property(e => e.RevokeReason)
                .HasMaxLength(255)
                .HasColumnName("revoke_reason");
            entity.Property(e => e.Revoked)
                .HasDefaultValue(false)
                .HasColumnName("revoked");
            entity.Property(e => e.TokenHash).HasColumnName("token_hash");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_tokens_users_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
