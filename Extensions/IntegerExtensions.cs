namespace PromoSeeker
{
    /// <summary>
    /// The extensions for the <see cref="int"/> data type.
    /// </summary>
    public static class IntegersExtension
    {
        /// <summary>
        /// Checks whether the integer value is between minimal and maximal value.
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns><see langword="true"/> if the value is between specified <paramref name="min"/> and <paramref name="max"/> values, otherwise <see langword="false"/>.</returns>
        public static bool InRange(this int integer, int min, int max)
        {
            return integer >= min && integer <= max;
        }
    }
}
