using System.Reflection;

namespace DataGen
{
    internal class RandomResourceData : IRandomData
    {        
        private readonly string _resourceName;
        private readonly string[] _data;        
        private readonly int _dataLength;

        public RandomResourceData(string resourceName, Random random)
        {            
            _resourceName = resourceName;
            _data = GetStringArrayResource($"DataGen.Resources.{resourceName}");
            _dataLength = _data.Length;
            Random = random;
        }

        public Random Random { get; set; }

        private string[] GetStringArrayResource(string resourceName)
        {
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

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
            return Prepend() + _data[Random.Next(_dataLength)] + Append();
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
