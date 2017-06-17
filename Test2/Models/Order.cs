using Postulate.Orm.Abstract;
using Postulate.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2.Models
{
    public class Order : Record<int>
    {
        [ForeignKey(typeof(Organization))]
        [PrimaryKey]
        public int OrganizationId { get; set; }

        [MaxLength(20)]
        [PrimaryKey]
        public string Number { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey(typeof(Customer))]
        public int CustomerId { get; set; }
    }
}
