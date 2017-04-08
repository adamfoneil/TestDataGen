using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.Models;
using TestData;
using Dapper;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();            
            
            using (SqlConnection cn = new SqlConnection("whatever"))
            {
                cn.Open();
                var tdg = new TestDataGenerator(cn, cts.Token);

                tdg.Generate<Organization>(3, org =>
                {
                    org.Name = tdg.Random(Source.CompanyName);
                }, (connection, records) =>
                {
                    // save org
                });

                var orgIds = cn.Query<int>("SELECT [Id] FROM [Organization]");

                foreach (int orgId in orgIds)
                {
                    tdg.Generate<FeeSchedule>(1, 4, fs =>
                    {
                        fs.OrganizationId = orgId;
                        fs.Name = tdg.Random(Source.WidgetName);
                    }, (connection, records) =>
                    {
                        // save fee schedule
                    });

                    var feeSchedules = cn.Query<FeeSchedule>("SELECT * FROM [dbo].[FeeSchedule]").ToArray();

                    tdg.Generate<Person>(750, 1500, p =>
                    {
                        p.OrganizationId = orgId;
                        p.FeeScheduleId = tdg.Random(feeSchedules, (fs) => fs.Id, (fs) => fs.OrganizationId == orgId);
                        p.FirstName = tdg.Random(Source.FirstName);
                        p.LastName = tdg.Random(Source.LastName);
                        p.Address = tdg.Random(Source.Address, 15);
                        p.City = tdg.Random(Source.City, 15);
                        p.State = tdg.Random(Source.USState);
                        p.ZipCode = tdg.Random(Source.USZipCode);
                        p.HomePhone = tdg.Random(Source.USPhoneNumber, 35);
                        p.WorkPhone = tdg.Random(Source.USPhoneNumber, 50);
                    }, (connection, records) =>
                    {
                        // save person
                    });
                }
            }
        }
    }
}
