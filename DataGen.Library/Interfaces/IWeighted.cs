namespace DataGen
{
    public interface IWeighted
    {
        /// <summary>
        /// Relative frequency of this item
        /// </summary>       
        int Factor { get; set; }

        /// <summary>
        /// Do not set this value yourself. It's set automatically to the sum of IWeighted.Factor values to determine the lo-bound for this bucket
        /// </summary>
        int MinBucketValue { get; set; }

        /// <summary>
        /// Do not set this value yourself. It's set automatically to the sum of IWeighted.Factor values to determine the hi-bound for this bucket
        /// </summary>
        int MaxBucketValue { get; set; }
    }
}
