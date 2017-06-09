using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamOneilSoftware
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
