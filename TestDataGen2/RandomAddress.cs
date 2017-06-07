using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestData
{
    internal class RandomAddress : RandomResourceData
    {
        public RandomAddress() : base("TestData.Resources.StreetNames.txt")
        {
        }

        protected override string Prepend()
        {
            return _rnd.Next(5000).ToString() + " ";
        }
    }
}
