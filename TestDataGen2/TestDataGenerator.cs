using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestData
{
    public enum Source
    {
        FirstName,
        LastName,
        Address,
        City,
        USState,
        USZipCode,
        USZipCodePlus4,
        USPhoneNumber,
        Email,
        CompanyName,
        WidgetName
    }

    public class TestDataGenerator
    {
        private readonly IDbConnection _connection;
        private readonly Random _rnd;

        public TestDataGenerator(IDbConnection connection)
        {
            _connection = connection;
            _rnd = new Random();
        }

        public int BatchSize { get; set; } = 50;

        public void Generate<TModel>(int recordCount, Action<TModel> create, Action<IDbConnection, IEnumerable<TModel>> save) where TModel : new()
        {
            List<TModel> records = new List<TModel>();
            int recordNum = 0;

            for (int i = 0; i < recordCount; i++)
            {
                recordNum++;
                TModel record = new TModel();
                create.Invoke(record);
                records.Add(record);

                if (recordNum == BatchSize)
                {
                    save.Invoke(_connection, records);
                    records.Clear();
                    recordNum = 0;
                }
            }

            // any leftovers
            save.Invoke(_connection, records);
        }

        public void Generate<TModel>(int minRecordCount, int maxRecordCount, Action<TModel> create, Action<IDbConnection, IEnumerable<TModel>> save) where TModel : new()
        {
            int recordCount = _rnd.Next(minRecordCount, maxRecordCount);
            Generate(recordCount, create, save);
        }

        public string Random(Source source, int nullFrequency = 0)
        {
            throw new NotImplementedException();
        }

        public T RandomLookup<T>(string query, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}
