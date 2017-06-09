using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamOneilSoftware
{
    internal interface IRandomData
    {
        Random Random { get; set; }
        string GetData();
    }
}
