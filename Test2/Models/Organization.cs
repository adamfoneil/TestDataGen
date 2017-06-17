using Postulate.Orm.Abstract;
using Postulate.Orm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Test2.Models
{
    public class Organization : Record<int>
    {
        [MaxLength(100)]
        [PrimaryKey]
        public string Name { get; set; }
        
        public decimal TaxRate { get; set; }
    }
}
