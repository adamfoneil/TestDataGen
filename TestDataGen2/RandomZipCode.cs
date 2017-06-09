using System;

namespace AdamOneilSoftware
{
    internal class RandomZipCode : IRandomData
    {
        public RandomZipCode(Random random)
        {
            Random = random;
        }

        public Random Random { get; set; }

        public string GetData()
        {
            return TestDataGenerator.GetRandomString(Random, "0123456789", 5);
        }
    }
}