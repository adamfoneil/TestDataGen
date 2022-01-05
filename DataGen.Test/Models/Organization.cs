using System.ComponentModel.DataAnnotations;

namespace DataGen.Test.Models
{
    public class Organization
    {
        [MaxLength(100)]        
        public string Name { get; set; }
        
        public decimal TaxRate { get; set; }
    }
}
