namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// UserCompany junction entity for multi-company support
    /// </summary>
    public class UserCompany : BaseEntity
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public bool IsPrimary { get; set; } = false;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
}
