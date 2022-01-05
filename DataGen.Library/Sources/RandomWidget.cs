using System;

namespace DataGen
{
    internal class RandomWidget : RandomResourceData
    {
        public RandomWidget(Random rnd) : base("Widgets.txt", rnd)
        {
        }

        protected override string Append()
        {
            var rndString = new RandomFormattedString(Random) { Format = "-000"};
            return rndString.GetData();
        }
    }
}
