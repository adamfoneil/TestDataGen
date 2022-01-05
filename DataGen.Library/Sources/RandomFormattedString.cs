using System;
using System.Collections.Generic;

namespace DataGen
{
    internal class RandomFormattedString : IRandomData
    {
        private Random random;

        public RandomFormattedString(Random rnd)
        {
            Random = rnd;            
        }

        public Random Random { get; set; }
        public string Format { get; set; }

        public string GetData()
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            Dictionary<char, string> charSets = new Dictionary<char, string>()
            {
                { 'A', alphabet.ToUpper() },
                { 'a', alphabet.ToLower() },
                { 'N', alphabet.ToUpper() + numbers },
                { 'n', alphabet.ToLower() + numbers },
                { '0', numbers }
            };

            Func<char, bool> isControlChar = (c) => { return charSets.ContainsKey(c); };

            string result = string.Empty;

            for (int i = 0; i < Format.Length; i++)
            {
                char c = Format[i];
                if (isControlChar(c))
                {
                    string charSet = charSets[c];
                    int index = Random.Next(charSet.Length);
                    result += charSet.Substring(index, 1);
                }
                else
                {
                    result += c;
                }
            }

            return result;
        }
    }
}