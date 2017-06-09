using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AdamOneilSoftware
{
    internal class RandomResourceData
    {
        protected readonly Random _rnd;

        private readonly string _resourceName;
        private readonly string[] _data;        
        private readonly int _dataLength;

        public RandomResourceData(string resourceName)
        {
            _rnd = new Random();
            _resourceName = resourceName;
            _data = GetStringArrayResource(resourceName);
            _dataLength = _data.Length;
        }

        private string[] GetStringArrayResource(string resourceName)
        {
            List<string> results = new List<string>();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream) results.Add(reader.ReadLine());
                }
            }
            return results.ToArray();
        }

        public string GetData()
        {            
            return Prepend() + _data[_rnd.Next(_dataLength - 1)] + Append();
        }

        protected virtual string Prepend()
        {
            return string.Empty;
        }

        protected virtual string Append()
        {
            return string.Empty;
        }
    }
}
