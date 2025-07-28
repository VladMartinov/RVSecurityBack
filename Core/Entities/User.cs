namespace Core.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string NormalizedUserName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<UserEmail> UserEmails { get; set; } = new List<UserEmail>();

    public virtual UserPhone? UserPhone { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}
