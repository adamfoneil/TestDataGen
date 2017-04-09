﻿using System;
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
            TestDataGenerator tdg = new TestDataGenerator(cts.Token);

            tdg.Generate<Person2>(100, 
                p =>
                {
                    p.FirstName = tdg.Random(Source.FirstName, 10);
                    p.LastName = tdg.Random(Source.LastName);
                    //p.Sex = tdg.Random(new char[] { 'M', 'F' }, c => c);
                    p.Sex = tdg.RandomWeighted(new SexWeighted[]
                    {
                        new SexWeighted() { Letter = 'M', Factor = 30 },
                        new SexWeighted() { Letter = 'F', Factor = 1 }
                    }, m => m.Letter);

                }, records =>
                {
                    Console.WriteLine("--- batch ---");
                    foreach (var record in records)
                    {
                        Console.WriteLine($"first = {record.FirstName}, last = {record.LastName}, sex = {record.Sex}");
                    }
                });

            Console.ReadLine();
        }
    }
}
