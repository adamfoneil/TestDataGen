using DataGen.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            List<Organization> orgs = new();
            List<Customer> customers = new();

            await dg.GenerateAsync(3, (index) => new Organization()
            {
                Id = index + 1,
                Name = dg.Random(Source.CompanyName),
                TaxRate = dg.Random(new [] { 0m, 0.1m, 0.13m, 0.04m }),
                
            }, async (rows) =>
            {
                orgs.AddRange(rows);                
                await Task.CompletedTask;
            });

            var orgIds = orgs.Select(o => o.Id).ToArray();

            await dg.GenerateAsync(300, (index) =>
            {
                var firstName = dg.Random(Source.FirstName);
                var lastName = dg.Random(Source.LastName);
                return new Customer()
                {
                    Id = index + 1,
                    OrganizationId = dg.Random(orgIds),
                    FirstName = firstName,
                    LastName = lastName,
                    Address = dg.Random(Source.Address),
                    City = dg.Random(Source.City),
                    State = dg.Random(Source.USState),
                    ZipCode = dg.Random(Source.USZipCode),      
                    PhoneNumber = dg.Random(Source.USPhoneNumber),
                    Email = $"{firstName}.{lastName}@{dg.Random(Source.DomainName)}"
                };
            }, async (rows) =>
            {
                customers.AddRange(rows);
                await Task.CompletedTask;
            });

            foreach (var customer in customers) Debug.Print(customer.ToString());
        }
    }
}