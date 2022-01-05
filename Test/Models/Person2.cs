using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGen;

namespace Test.Models
{
    public class Person2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char Sex { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ItemName { get; set; }
        public DateTime SomeDate { get; set; }
    }

    public class SexWeighted : IWeighted
    {       
        public char Letter { get; set; }

        // IWeighted members
        public int Factor { get; set; }
        public int MaxBucketValue { get; set; }
        public int MinBucketValue { get; set; }
    }
}
