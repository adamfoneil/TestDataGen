using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace AdamOneilSoftware
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
        DomainName,
        CompanyName,
        WidgetName
    }

    public class TestDataGenerator
    {
        private readonly Random _rnd;
        private readonly CancellationToken _cancellationToken;

        private Dictionary<Source, IRandomData> _randomSources = null;
        private RandomFormattedString _rndFormatted;

        public TestDataGenerator(CancellationToken cancellationToken = default(CancellationToken))
        {            
            _rnd = new Random();
            _cancellationToken = cancellationToken;

            _randomSources = new Dictionary<Source, IRandomData>()
            {
                { Source.FirstName, new RandomResourceData("FirstNames.txt", _rnd) },
                { Source.LastName, new RandomResourceData("LastNames.txt", _rnd) },
                { Source.Address, new RandomAddress(_rnd) },
                { Source.City, new RandomResourceData("Cities.txt", _rnd) },
                { Source.USState, new RandomResourceData("USStates.txt", _rnd) },
                { Source.USZipCode, new RandomFormattedString(_rnd) { Format = "00000" } },
                { Source.USZipCodePlus4, new RandomFormattedString(_rnd) { Format = "00000-0000" } },
                { Source.USPhoneNumber, new RandomFormattedString(_rnd) { Format = "(000) 000-0000" } },
                { Source.WidgetName, new RandomWidget(_rnd) },
                { Source.CompanyName, new RandomResourceData("CompanyNames.txt", _rnd) },
                { Source.DomainName, new RandomResourceData("DomainNames.txt", _rnd) }
            };
        }

        public int BatchSize { get; set; } = 50;        

        public void Generate<TModel>(int recordCount, Action<TModel> create, Action<IEnumerable<TModel>> save) where TModel : new()
        {
            _rndFormatted = new RandomFormattedString(_rnd);

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

            if (_cancellationToken.IsCancellationRequested) return;

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

        /// <summary>
        /// Generates a number of randomg records, checking to see if they violate a key before saving
        /// </summary>
        public void GenerateUnique<TModel>(IDbConnection connection, int recordCount, Action<TModel> create, Func<IDbConnection, TModel, bool> exists, Action<TModel> save) where TModel : new()
        {
            _rndFormatted = new RandomFormattedString(_rnd);
           
            for (int i = 0; i < recordCount; i++)
            {
                if (_cancellationToken.IsCancellationRequested) break;

                TModel record = new TModel();                
                do
                {
                    create.Invoke(record);
                } while (exists.Invoke(connection, record));
                save.Invoke(record);
            }
        }

        public void GenerateUnique<TModel>(IDbConnection connection, int minRecordCount, int maxRecordCount, Action<TModel> create, Func<IDbConnection, TModel, bool> exists, Action<TModel> save) where TModel : new()
        {
            int records = _rnd.Next(minRecordCount, maxRecordCount);
            GenerateUnique(connection, records, create, exists, save);
        }

        public TValue RandomWeighted<TValue, TModel>(TModel[] source, Func<TModel, TValue> select, Func<TModel, bool> predicate = null, int nullFrequency = 0) where TModel : IWeighted
        {
            var values = (predicate != null) ?
                source.Where(row => predicate.Invoke(row)).ToArray() :
                source;

            TModel priorItem = default(TModel);
            foreach (var item in values)
            {                
                item.MinBucketValue = (priorItem != null) ? priorItem.MaxBucketValue + 1 : 0;
                item.MaxBucketValue = item.MinBucketValue + item.Factor;
                priorItem = item;
            }

            int totalFactor = priorItem.MaxBucketValue;

            if (IsRandomNull(nullFrequency))
            {
                return default(TValue);
            }
            else
            {
                int weightedIndex = _rnd.Next(0, totalFactor);
                return select(values.First(item => item.MinBucketValue <= weightedIndex && weightedIndex <= item.MaxBucketValue));
            }
        }

        /// <summary>
        /// Returns a random value from an array of TModel with an optional predicate and null frequency precentage
        /// </summary>
        /// <typeparam name="TValue">Type to return</typeparam>
        /// <typeparam name="TModel">Type of possible values to select from</typeparam>
        /// <param name="source">An array of TModel to select from</param>
        /// <param name="select">Expression of type TValue that returns the property of TModel</param>
        /// <param name="predicate">Optional -- filter to apply to source to restrict selection to a subset of source</param>
        /// <param name="nullFrequency">Number between 0 and 99 that indicates how often null is returned</param>        
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

        public TValue Random<TValue>(TValue[] source, Func<TValue, bool> predicate = null, int nullFrequency = 0)
        {
            return Random(source, item => item, predicate, nullFrequency);
        }

        /// <summary>
        /// Returns a random int within a specified min/max range with a specified null frequency
        /// </summary>
        public int? RandomInRange(int min, int max, int nullFrequency = 0)
        {
            if (!IsRandomNull(nullFrequency))
            {
                var range = Enumerable.Range(min, max).ToArray();
                return Random(range, null, nullFrequency);
            }
            else
            {
                return null;
            }            
        }

        /// <summary>
        /// Returns a random value of any type that can be derived from an int with a specified null frequency
        /// </summary>
        public TValue RandomInRange<TValue>(int min, int max, Func<int, TValue> createValue, int nullFrequency = 0)
        {
            if (!IsRandomNull(nullFrequency))
            {
                var range = Enumerable.Range(min, max).Select(i => createValue.Invoke(i)).ToArray();
                return Random(range, null, nullFrequency);
            }
            else
            {
                return default(TValue);
            }
        }

        private bool IsRandomNull(int nullFrequency)
        {
            if (nullFrequency == 0) return false;
            if (nullFrequency < 0) throw new ArgumentException("NullFrequency cannot be less than zero.");
            if (nullFrequency > 99) throw new ArgumentException("NullFrequency cannot be greater than 99.");

            int value = _rnd.Next(100);
            return (value < nullFrequency);
        }

        public string Random(Source source, int nullFrequency = 0)
        {
            if (IsRandomNull(nullFrequency))
            {
                return null;
            }
            else
            {
                return _randomSources[source].GetData();
            }
        }

        public string RandomFormatted(string format, int nullFrequency = 0)
        {
            if (IsRandomNull(nullFrequency))
            {
                return null;
            }
            else
            {
                _rndFormatted.Format = format;
                return _rndFormatted.GetData();
            }
        }
    }
}
