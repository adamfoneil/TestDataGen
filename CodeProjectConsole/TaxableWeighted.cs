using AdamOneilSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    internal class TaxableWeighted : IWeighted
    {
        public bool IsTaxable { get; set; }

        public int Factor { get; set; }
        public int MinBucketValue { get; set; }
        public int MaxBucketValue { get; set; }
    }
}
