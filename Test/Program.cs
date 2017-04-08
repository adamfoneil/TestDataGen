using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models;
using TestData;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection cn = new SqlConnection("whatever"))
            {
                cn.Open();
                var tdg = new TestDataGenerator(cn);

                tdg.Generate<Organization>(3, org =>
                {
                    org.Name = tdg.Random(Source.CompanyName);

                    tdg.Generate<FeeSchedule>(1, 4, fs =>
                    {
                        fs.OrganizationId = org.Id;
                        fs.Name = tdg.Random(Source.WidgetName);
                    }, (connection, records) =>
                    {
                        // save fee schedule
                    });

                    tdg.Generate<Person>(850, 1250, p =>
                    {
                        p.OrganizationId = org.Id;
                        p.FeeScheduleId = tdg.RandomLookup<int>("SELECT [Id] FROM [dbo].[FeeSchedule] WHERE [OrganizationId]=@orgId", new { orgId = org.Id });
                        p.FirstName = tdg.Random(Source.FirstName);
                        p.LastName = tdg.Random(Source.LastName);
                        p.Address = tdg.Random(Source.Address, 15);
                        p.City = tdg.Random(Source.City, 15);
                        p.State = tdg.Random(Source.USState);
                        p.ZipCode = tdg.Random(Source.USZipCode);
                    }, (connection, records) =>
                    {
                        // save person
                    });

                }, (connection, records) =>
                {
                    // save org
                });
            }
        }
    }
}
