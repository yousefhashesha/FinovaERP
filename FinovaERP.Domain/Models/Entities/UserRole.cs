namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// UserRole junction entity for many-to-many relationship
    /// </summary>
    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public string? AssignedBy { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
}
