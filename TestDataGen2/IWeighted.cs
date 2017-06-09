using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamOneilSoftware
{
    public interface IWeighted
    {
        int Factor { get; set; }
        int MinBucketValue { get; set; }
        int MaxBucketValue { get; set; }
    }
}
