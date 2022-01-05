using System;

namespace DataGen
{
    internal class RandomAddress : RandomResourceData
    {
        public RandomAddress(Random random) : base("StreetNames.txt", random)
        {
        }

        protected override string Prepend()
        {
            return Random.Next(5000).ToString() + " ";
        }
    }
}
