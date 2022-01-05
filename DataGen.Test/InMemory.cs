using DataGen.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataGen.Test
{
    [TestClass]
    public class InMemory
    {
        [TestMethod]
        public async Task SimpleCase()
        {
            var dg = new DataGenerator() { BatchSize = 10 };

            await dg.GenerateAsync(3, () => new Organization()
            {
                Name = dg.Random(Source.CompanyName),
                TaxRate = dg.Random(new [] { 0m, 0.1m, 0.13m, 0.04m })
            }, async (rows) =>
            {
                foreach (var row in rows)
                {
                    Debug.Print(row.Name);
                }

                await Task.CompletedTask;
            });
        }
    }
}