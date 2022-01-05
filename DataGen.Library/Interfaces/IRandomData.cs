using System;

namespace DataGen
{
    internal interface IRandomData
    {
        Random Random { get; set; }
        string GetData();
    }
}
