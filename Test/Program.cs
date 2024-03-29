﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.Models;
using DataGen;
using Dapper;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {            
            TestDataGenerator tdg = new TestDataGenerator();

            tdg.Generate<Person2>(100, 
                p =>
                {
                    p.FirstName = tdg.Random(Source.FirstName, 10);
                    p.LastName = tdg.Random(Source.LastName);
                    //p.Sex = tdg.Random(new char[] { 'M', 'F' });
                    p.State = tdg.Random(Source.USState);
                    p.ZipCode = tdg.Random(Source.USZipCode);
                    p.Phone = tdg.RandomFormatted("000.0000");
                    p.ItemName = tdg.Random(Source.WidgetName);
                    if (p.FirstName != null)
                    {
                        p.Email = $"{p.FirstName.ToLower()}.{p.LastName.ToLower()}@{tdg.Random(Source.DomainName)}";
                    }                    
                    p.Sex = tdg.RandomWeighted(new SexWeighted[]
                    {
                        new SexWeighted() { Letter = 'M', Factor = 3 },
                        new SexWeighted() { Letter = 'F', Factor = 1 }
                    }, m => m.Letter);
                    p.SomeDate = tdg.RandomInRange(0, 30, i => new DateTime(2003, 1, 1).AddDays(i));

                }, records =>
                {
                    Console.WriteLine("--- batch ---");
                    foreach (var record in records)
                    {
                        Console.WriteLine($"first = {record.FirstName}, last = {record.LastName}, email = {record.Email}, someDate = {record.SomeDate}");
                    }
                });

            Console.ReadLine();
        }
    }
}
