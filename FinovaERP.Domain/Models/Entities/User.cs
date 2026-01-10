using System;
using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// User account with authentication and profile information
    /// </summary>
    public class User : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? ProfilePicture { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;

        public DateTime? LastLoginDate { get; set; }

        public bool IsLocked { get; set; } = false;

        public DateTime? LockoutEndDate { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public string FullName => $""{FirstName} {LastName}"".Trim();
        public bool IsLockedOut => IsLocked && LockoutEndDate.HasValue && LockoutEndDate > DateTime.UtcNow;
    }
}
