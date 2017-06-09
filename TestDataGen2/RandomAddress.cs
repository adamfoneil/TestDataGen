using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamOneilSoftware
{
    internal class RandomAddress : RandomResourceData
    {
        public RandomAddress(Random random) : base("AdamOneilSoftware.Resources.StreetNames.txt", random)
        {
        }

        protected override string Prepend()
        {
            return Random.Next(5000).ToString() + " ";
        }
    }
}
