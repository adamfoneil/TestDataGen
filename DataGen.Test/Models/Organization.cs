using System.ComponentModel.DataAnnotations;

namespace DataGen.Test.Models
{
    public class Organization
    {
        public int Id { get; set; }

        [MaxLength(100)]        
        public string Name { get; set; }
        
        public decimal TaxRate { get; set; }        
    }
}
