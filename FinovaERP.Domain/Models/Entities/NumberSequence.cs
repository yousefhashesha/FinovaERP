using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Number sequence for automatic code generation
    /// </summary>
    public class NumberSequence : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(10)]
        public string? Prefix { get; set; }

        [StringLength(10)]
        public string? Suffix { get; set; }

        public int LastNumber { get; set; }

        [Range(1, 10)]
        public int MinDigits { get; set; } = 4;

        public string FormatNumber(int number)
        {
            string formattedNumber = number.ToString().PadLeft(MinDigits, '0');
            
            if (!string.IsNullOrEmpty(Prefix))
                formattedNumber = Prefix + formattedNumber;
                
            if (!string.IsNullOrEmpty(Suffix))
                formattedNumber = formattedNumber + Suffix;
                
            return formattedNumber;
        }

        public string GetNextNumber()
        {
            LastNumber++;
            return FormatNumber(LastNumber);
        }
    }
}
