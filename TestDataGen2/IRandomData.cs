using System;

namespace AdamOneilSoftware
{
    internal interface IRandomData
    {
        Random Random { get; set; }
        string GetData();
    }
}
