using System.Data;

namespace DataGen
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
        WidgetName,
        UniqueWidget
    }

    public class DataGenerator
    {
        private readonly Random _rnd;
        private readonly CancellationToken _cancellationToken;

        private Dictionary<Source, IRandomData> _randomSources;
        private RandomFormattedString _rndFormatted;

        public DataGenerator(CancellationToken cancellationToken = default(CancellationToken))
        {            
            _rnd = new Random();
            _cancellationToken = cancellationToken;

            _randomSources = new Dictionary<Source, IRandomData>()
            {
                [Source.FirstName] = new RandomResourceData("FirstNames.txt", _rnd),
                [Source.LastName] = new RandomResourceData("LastNames.txt", _rnd),
                [Source.Address] = new RandomAddress(_rnd),
                [Source.City] = new RandomResourceData("Cities.txt", _rnd),
                [Source.USState] = new RandomResourceData("USStates.txt", _rnd),
                [Source.USZipCode] = new RandomFormattedString(_rnd) { Format = "00000" },
                [Source.USZipCodePlus4] = new RandomFormattedString(_rnd) { Format = "00000-0000" },
                [Source.USPhoneNumber] = new RandomFormattedString(_rnd) { Format = "(000) 000-0000" },
                [Source.UniqueWidget] = new RandomWidget(_rnd),
                [Source.WidgetName] = new RandomResourceData("Widgets.txt", _rnd),
                [Source.CompanyName] = new RandomResourceData("CompanyNames.txt", _rnd),
                [Source.DomainName] = new RandomResourceData("DomainNames.txt", _rnd)
            };
        }

        public int BatchSize { get; set; } = 50;        

        public async Task GenerateAsync<TModel>(int recordCount, Func<int, TModel> create, Func<IEnumerable<TModel>, Task> save) where TModel : new()
        {
            _rndFormatted = new RandomFormattedString(_rnd);

            List<TModel> records = new List<TModel>(BatchSize);
            int recordNum = 0;

            for (int i = 0; i < recordCount; i++)
            {
                if (_cancellationToken.IsCancellationRequested) break;

                recordNum++;
                TModel record = create.Invoke(i);
                records.Add(record);

                if (recordNum == BatchSize)
                {
                    await save.Invoke(records);
                    records.Clear();
                    recordNum = 0;
                }
            }

            if (_cancellationToken.IsCancellationRequested) return;

            // any leftovers
            await save.Invoke(records);
        }

        /// <summary>
        /// Generates a random number of records between the min and max you specify
        /// </summary>
        /// <typeparam name="TModel">Type of model class to create</typeparam>
        /// <param name="minRecordCount">Low bound of random record count</param>
        /// <param name="maxRecordCount">Hi bound of random record count</param>
        /// <param name="create">Action that initializes the generated record</param>
        /// <param name="save">Action that saves the generated records to the database according to BatchSize</param>
        public async Task GenerateAsync<TModel>(int minRecordCount, int maxRecordCount, Func<int, TModel> create, Func<IEnumerable<TModel>, Task> save) where TModel : new()
        {
            int recordCount = _rnd.Next(minRecordCount, maxRecordCount);
            await GenerateAsync(recordCount, create, save);
        }

        /// <summary>
        /// Generates a number of random records, checking to see if they violate a key before saving
        /// </summary>
        public async Task GenerateUniqueAsync<TModel>(IDbConnection connection, int recordCount, Action<TModel> create, Func<IDbConnection, TModel, Task<bool>> exists, Func<TModel, Task> save) where TModel : new()
        {
            _rndFormatted = new RandomFormattedString(_rnd);
           
            for (int i = 0; i < recordCount; i++)
            {
                if (_cancellationToken.IsCancellationRequested) break;

                TModel record = new TModel();
                int attempts = 0;
                const int maxAttempts = 100;

                do
                {
                    attempts++;
                    if (attempts > maxAttempts) throw new Exception($"Couldn't generate a random {typeof(TModel).Name} record after {maxAttempts} tries. Exception was thrown to prevent infinite loop.");
                    create.Invoke(record);                    
                } while (await exists.Invoke(connection, record));
                
                await save.Invoke(record);
            }
        }

        public async Task GenerateUniqueAsync<TModel>(IDbConnection connection, int minRecordCount, int maxRecordCount, Action<TModel> create, Func<IDbConnection, TModel, Task<bool>> exists, Func<TModel, Task> save) where TModel : new()
        {
            int records = _rnd.Next(minRecordCount, maxRecordCount);
            await GenerateUniqueAsync(connection, records, create, exists, save);
        }

        public async Task GenerateUpToAsync<TModel>(IDbConnection connection, int maxCount, Func<IDbConnection, Task<int>> getRecordCount, Func<int, TModel> create, Func<IEnumerable<TModel>, Task> save) where TModel : new()
        {
            int currentCount = await getRecordCount.Invoke(connection);
            int generate = maxCount - currentCount;
            if (generate > 0) await GenerateAsync(generate, create, save);
        }

        public async Task GenerateUniqueUpToAsync<TModel>(IDbConnection connection, int maxCount, Func<IDbConnection, Task<int>> getRecordCount, Action<TModel> create, Func<IDbConnection, TModel, Task<bool>> exists, Func<TModel, Task> save) where TModel : new()
        {
            int currentCount = await getRecordCount.Invoke(connection);
            int generate = maxCount - currentCount;
            if (generate > 0) await GenerateUniqueAsync(connection, generate, create, exists, save);
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
                return default;
            }
            else
            {
                _rndFormatted.Format = format;
                return _rndFormatted.GetData();
            }
        }
    }
}
