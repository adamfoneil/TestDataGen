using Postulate.Orm.Abstract;
using Postulate.Orm.Attributes;

namespace Test2.Models
{
    public class OrderItem : Record<int>
    {
        [ForeignKey(typeof(Order))]
        [PrimaryKey]
        public int OrderId { get; set; }

        [ForeignKey(typeof(Item))]
        [PrimaryKey]
        public int ItemId { get; set; }

        public int Quantity { get; set; }
        
        [DecimalPrecision(7,2)]
        public decimal UnitPrice { get; set; }

        [DecimalPrecision(7, 2)]
        public decimal ExtPrice { get; set; }
    }
}
