using Postulate.Orm.Abstract;
using Postulate.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public decimal UnitPrice { get; set; }

        public decimal ExtPrice { get; set; }
    }
}
