using System.ComponentModel.DataAnnotations;

namespace DataGen.Test.Models
{
    public class Item
    {
        public int OrganizationId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public decimal UnitCost { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsTaxable { get; set; }
    }
}
