using AdamOneilSoftware;
using Dapper;
using Postulate.Orm.Extensions;
using System;
using System.Linq;
using Test2.Models;

namespace Test2
{
    class Program
    {
        private static TestDataGenerator _tdg = new TestDataGenerator();
        private static TdgDb _db = new TdgDb();

        static void Main(string[] args)
        {
            //CreateOrgs();
            //CreateItems();
            //CreateCustomers();
            //CreateOrders();
            CreateOrderItems();
            
        }

        private static void CreateOrderItems()
        {
            int[] orgIds = null;
            dynamic[] items = null;
            dynamic[] orders = null;

            using (var cn = _db.GetConnection())
            {
                cn.Open();
                orgIds = cn.Query<int>("SELECT [Id] FROM [Organization]").ToArray();
                items = cn.Query("SELECT [OrganizationId], [Id] FROM [Item]").ToArray();
                orders = cn.Query("SELECT [OrganizationId], [Id] FROM [Order]").ToArray();
                
                foreach (var order in orders)
                {
                    _tdg.GenerateUnique<OrderItem>(cn, 1, 7, oi =>
                    {
                        oi.OrderId = order.Id;
                        oi.ItemId = _tdg.Random(items, item => item.Id, item => item.OrganizationId == order.OrganizationId);
                        oi.Quantity = _tdg.RandomInRange(1, 25).Value;
                        oi.UnitPrice = _db.Find<Item>(cn, oi.ItemId).UnitPrice;
                        oi.ExtPrice = oi.Quantity * oi.UnitPrice;
                    }, (connection, record) =>
                    {
                        return connection.Exists(
                            "[OrderItem] WHERE [OrderId]=@orderId AND [ItemId]=@itemId", 
                            new { orderId = record.OrderId, itemId = record.ItemId });
                    }, (record) =>
                    {
                        _db.Save(record);
                    });
                }
            }
        }

        private static void CreateOrders()
        {
            int[] orgIds = null;
            dynamic[] customerIds = null;

            using (var cn = _db.GetConnection())
            {
                cn.Open();
                orgIds = cn.Query<int>("SELECT [Id] FROM [Organization]").ToArray();
                customerIds = cn.Query("SELECT [OrganizationId], [Id] FROM [Customer]").ToArray();
            }

            _tdg.Generate<Order>(7000, ord =>
            {
                ord.OrganizationId = _tdg.Random(orgIds);
                ord.CustomerId = _tdg.Random(customerIds, item => item.Id, item => item.OrganizationId == ord.OrganizationId);
                ord.Number = _tdg.RandomFormatted("AA000-A0000");
                ord.Date = _tdg.RandomInRange(0, 2000, i => new DateTime(2013, 1, 1).AddDays(i));
            }, records =>
            {
                _db.SaveMultiple(records);
            });
        }

        private static void CreateCustomers()
        {
            int[] orgIds = null;

            using (var cn = _db.GetConnection())
            {
                cn.Open();
                orgIds = cn.Query<int>("SELECT [Id] FROM [Organization]").ToArray();
            }

            _tdg.Generate<Customer>(5000, c =>
            {
                c.OrganizationId = _tdg.Random(orgIds);
                c.FirstName = _tdg.Random(Source.FirstName);
                c.LastName = _tdg.Random(Source.LastName);
                c.Address = _tdg.Random(Source.Address);
                c.City = _tdg.Random(Source.City);
                c.State = _tdg.Random(Source.USState);
                c.ZipCode = _tdg.Random(Source.USZipCode);
                c.Email = $"{c.FirstName.ToLower()}.{c.LastName.ToLower()}@{_tdg.Random(Source.DomainName)}";
            }, records =>
            {
                _db.SaveMultiple(records);
            });
        }

        private static void CreateItems()
        {
            int[] orgIds = null;

            using (var cn = _db.GetConnection())
            {
                cn.Open();
                orgIds = cn.Query<int>("SELECT [Id] FROM [Organization]").ToArray();
            }

            _tdg.Generate<Item>(250, item =>
            {
                item.OrganizationId = _tdg.Random(orgIds);
                item.Name = _tdg.Random(Source.WidgetName);
                item.UnitCost = Convert.ToDecimal(_tdg.RandomInRange(10, 150)) + _tdg.RandomInRange(10, 25, i => i * 0.1m);
                item.UnitPrice = item.UnitCost + Convert.ToDecimal(_tdg.RandomInRange(3, 50));
                item.IsTaxable = _tdg.RandomWeighted(new TaxableWeighted[]
                {
                    new TaxableWeighted() { IsTaxable = true, Factor = 1 },
                    new TaxableWeighted() { IsTaxable = false, Factor = 5 }
                }, x => x.IsTaxable);
            }, records =>
            {
                _db.SaveMultiple(records);
            });
        }

        private static void CreateOrgs()
        {
            _tdg.Generate<Organization>(10, org =>
            {
                org.Name = _tdg.Random(Source.CompanyName) + _tdg.RandomInRange(1, 100).ToString();
                org.TaxRate = _tdg.RandomInRange(0, 10, i => Convert.ToDecimal(i) * 0.01m);
            }, records =>
            {
                _db.SaveMultiple(records, 1);
            });
        }
    }
}
