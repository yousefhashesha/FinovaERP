namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Role entity for authorization
    /// </summary>
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Level { get; set; } = 1;
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
