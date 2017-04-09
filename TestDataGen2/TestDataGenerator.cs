using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
        private readonly CancellationToken _cancellationToken;

        public TestDataGenerator(IDbConnection connection, CancellationToken cancellationToken)
        {
            _connection = connection;
            _rnd = new Random();
            _cancellationToken = cancellationToken;
        }

        public int BatchSize { get; set; } = 50;
        public TimeSpan MaxRunTime { get; set; } = TimeSpan.FromMinutes(5);

        public void Generate<TModel>(int recordCount, Action<TModel> create, Action<IEnumerable<TModel>> save) where TModel : new()
        {
            List<TModel> records = new List<TModel>(BatchSize);
            int recordNum = 0;

            for (int i = 0; i < recordCount; i++)
            {
                if (_cancellationToken.IsCancellationRequested) break;

                recordNum++;
                TModel record = new TModel();
                create.Invoke(record);
                records.Add(record);

                if (recordNum == BatchSize)
                {
                    save.Invoke(records);
                    records.Clear();
                    recordNum = 0;
                }
            }

            // any leftovers
            save.Invoke(records);
        }

        /// <summary>
        /// Generates a random number of records between the min and max you specify
        /// </summary>
        /// <typeparam name="TModel">Type of model class to create</typeparam>
        /// <param name="minRecordCount">Low bound of random record count</param>
        /// <param name="maxRecordCount">Hi bound of random record count</param>
        /// <param name="create">Action that initializes the generated record</param>
        /// <param name="save">Action that saves the generated records to the database according to BatchSize</param>
        public void Generate<TModel>(int minRecordCount, int maxRecordCount, Action<TModel> create, Action<IEnumerable<TModel>> save) where TModel : new()
        {
            int recordCount = _rnd.Next(minRecordCount, maxRecordCount);
            Generate(recordCount, create, save);
        }

        public TValue RandomWeighted<TValue, TModel>(TModel[] source, Func<TModel, TValue> select, Func<TModel, bool> predicate = null, int nullFrequency = 0) where TModel : IWeighted
        {
            var values = (predicate != null) ?
                source.Where(row => predicate.Invoke(row)).ToArray() :
                source;

            TModel priorItem = default(TModel);
            foreach (var item in values)
            {                
                item.MinBucketValue = (priorItem != null) ? priorItem.MinBucketValue + 1 : 0;
                item.MaxBucketValue = item.MinBucketValue + item.Factor;
                priorItem = item;
            }

            int totalFactor = priorItem.MaxBucketValue;

            if (!IsRandomNull(nullFrequency))
            {
                int weightedIndex = _rnd.Next(0, totalFactor);
                return select(values.First(item => item.MinBucketValue <= weightedIndex && weightedIndex < item.MaxBucketValue));
            }
            else
            {
                return default(TValue);
            }
        }

        public TValue Random<TValue, TModel>(TModel[] source, Func<TModel, TValue> select, Func<TModel, bool> predicate = null, int nullFrequency = 0)
        {
            var values = (predicate != null) ?
                source.Where(row => predicate.Invoke(row)).ToArray() :
                source;

            if (!IsRandomNull(nullFrequency))
            {
                int index = _rnd.Next(0, values.Length);
                return select(values[index]);
            }
            else
            {
                return default(TValue);
            }            
        }

        private bool IsRandomNull(int nullFrequency)
        {
            throw new NotImplementedException();
        }

        public string Random(Source source, int nullFrequency = 0)
        {
            throw new NotImplementedException();
        }
    }
}
