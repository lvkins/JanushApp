using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoSeeker.Core.Algorithms
{
    // Right off the https://github.com/feature23/StringSimilarity.NET
    // Test comparison algorithms here: https://asecuritysite.com/forensics/simstring

    /// The Levenshtein distance between two words is the Minimum number of
    /// single-character edits (insertions, deletions or substitutions) required to
    /// change one string into the other.
    public class Levenshtein
    {
        /// <summary>
        /// The Levenshtein distance, or edit distance, between two words is the
        /// Minimum number of single-character edits (insertions, deletions or
        /// substitutions) required to change one word into the other.
        ///
        /// http://en.wikipedia.org/wiki/Levenshtein_distance
        ///
        /// It is always at least the difference of the sizes of the two strings.
        /// It is at most the length of the longer string.
        /// It is zero if and only if the strings are equal.
        /// If the strings are the same size, the HamMing distance is an upper bound
        /// on the Levenshtein distance.
        /// The Levenshtein distance verifies the triangle inequality (the distance
        /// between two strings is no greater than the sum Levenshtein distances from
        /// a third string).
        ///
        /// Implementation uses dynamic programMing (Wagner–Fischer algorithm), with
        /// only 2 rows of data. The space requirement is thus O(m) and the algorithm
        /// runs in O(mn).
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>The Levenshtein distance between strings</returns>
        /// <exception cref="ArgumentNullException">If s1 or s2 is null.</exception>
        public static double Distance(string s1, string s2)
        {
            if (s1 == null)
            {
                throw new ArgumentNullException(nameof(s1));
            }

            if (s2 == null)
            {
                throw new ArgumentNullException(nameof(s2));
            }

            if (s1.Equals(s2))
            {
                return 0;
            }

            if (s1.Length == 0)
            {
                return s2.Length;
            }

            if (s2.Length == 0)
            {
                return s1.Length;
            }

            // create two work vectors of integer distances
            var v0 = new int[s2.Length + 1];
            var v1 = new int[s2.Length + 1];
            int[] vtemp;

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (var i = 0; i < v0.Length; i++)
            {
                v0[i] = i;
            }

            for (var i = 0; i < s1.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0
                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (var j = 0; j < s2.Length; j++)
                {
                    var cost = 1;
                    if (s1[i] == s2[j])
                    {
                        cost = 0;
                    }
                    v1[j + 1] = Math.Min(
                            v1[j] + 1,              // Cost of insertion
                            Math.Min(
                                    v0[j + 1] + 1,  // Cost of remove
                                    v0[j] + cost)); // Cost of substitution
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                //System.arraycopy(v1, 0, v0, 0, v0.length);

                // Flip references to current and previous row
                vtemp = v0;
                v0 = v1;
                v1 = vtemp;
            }

            return v0[s2.Length];
        }

        /// <summary>
        /// Double similarity between two strings
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>double similarity</returns>
        public static double Similarity(string s1, string s2)
        {
            var dist = Distance(s1, s2);

            // No distance, strings are equal
            if (dist == 0)
            {
                return 1.0;
            }

            // Get longest string
            var max = s1.Length > s2.Length ? s1 : s2;

            // Get double difference
            return 1.0 - (dist / max.Length);
        }
    }

    /// <summary>
    /// Implementation of Damerau-Levenshtein distance with transposition (also 
    /// sometimes calls unrestricted Damerau-Levenshtein distance).
    /// It is the minimum number of operations needed to transform one string into
    /// the other, where an operation is defined as an insertion, deletion, or
    /// substitution of a single character, or a transposition of two adjacent
    /// characters.
    /// It does respect triangle inequality, and is thus a metric distance.
    /// This is not to be confused with the optimal string alignment distance, which
    /// is an extension where no substring can be edited more than once.
    /// </summary>
    public class DamerauLevenshtein
    {
        /// <summary>
        /// Compute the distance between strings: the minimum number of operations
        /// needed to transform one string into the other(insertion, deletion,
        /// substitution of a single character, or a transposition of two adjacent
        /// characters).
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>The computed distance.</returns>
        /// <exception cref="ArgumentNullException">If s1 or s2 is null.</exception>
        public double Distance(string s1, string s2)
        {
            if (s1 == null)
            {
                throw new ArgumentNullException(nameof(s1));
            }

            if (s2 == null)
            {
                throw new ArgumentNullException(nameof(s2));
            }

            if (s1.Equals(s2))
            {
                return 0;
            }

            // Infinite distance is the max possible distance
            var inf = s1.Length + s2.Length;

            // Create and initialize the character array indices
            var da = new Dictionary<char, int>();

            for (var d = 0; d < s1.Length; d++)
            {
                da[s1[d]] = 0;
            }

            for (var d = 0; d < s2.Length; d++)
            {
                da[s2[d]] = 0;
            }

            // Create the distance matrix H[0 .. s1.length+1][0 .. s2.length+1]
            var h = new int[s1.Length + 2, s2.Length + 2];

            // Initialize the left and top edges of H
            for (var i = 0; i <= s1.Length; i++)
            {
                h[i + 1, 0] = inf;
                h[i + 1, 1] = i;
            }

            for (var j = 0; j <= s2.Length; j++)
            {
                h[0, j + 1] = inf;
                h[1, j + 1] = j;
            }

            // Fill in the distance matrix H
            // Look at each character in s1
            for (var i = 1; i <= s1.Length; i++)
            {
                var db = 0;

                // Look at each character in b
                for (var j = 1; j <= s2.Length; j++)
                {
                    var i1 = da[s2[j - 1]];
                    var j1 = db;

                    var cost = 1;
                    if (s1[i - 1] == s2[j - 1])
                    {
                        cost = 0;
                        db = j;
                    }

                    h[i + 1, j + 1] = Min(
                        h[i, j] + cost,  // Substitution
                        h[i + 1, j] + 1, // Insertion
                        h[i, j + 1] + 1, // Deletion
                        h[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1)
                    );
                }

                da[s1[i - 1]] = i;
            }

            return h[s1.Length + 1, s2.Length + 1];
        }

        private static int Min(int a, int b, int c, int d)
             => Math.Min(a, Math.Min(b, Math.Min(c, d)));
    }

    /// The Jaro–Winkler distance metric is designed and best suited for short
    /// strings such as person names, and to detect typos; it is (roughly) a
    /// variation of Damerau-Levenshtein, where the substitution of 2 close
    /// characters is considered less important then the substitution of 2 characters
    /// that a far from each other.
    /// Jaro-Winkler was developed in the area of record linkage (duplicate
    /// detection) (Winkler, 1990). It returns a value in the interval [0.0, 1.0].
    /// The distance is computed as 1 - Jaro-Winkler similarity.
    public class JaroWinkler
    {
        private const double DEFAULT_THRESHOLD = 0.7;
        private const int THREE = 3;
        private const double JW_COEF = 0.1;

        /// <summary>
        /// The current value of the threshold used for adding the Winkler bonus. The default value is 0.7.
        /// </summary>
        private double Threshold { get; }

        /// <summary>
        /// Creates a new instance with default threshold (0.7)
        /// </summary>
        public JaroWinkler()
        {
            Threshold = DEFAULT_THRESHOLD;
        }

        /// <summary>
        /// Creates a new instance with given threshold to determine when Winkler bonus should
        /// be used. Set threshold to a negative value to get the Jaro distance.
        /// </summary>
        /// <param name="threshold"></param>
        public JaroWinkler(double threshold)
        {
            Threshold = threshold;
        }

        /// <summary>
        /// Compute Jaro-Winkler similarity.
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>The Jaro-Winkler similarity in the range [0, 1]</returns>
        /// <exception cref="ArgumentNullException">If s1 or s2 is null.</exception>
        public double Similarity(string s1, string s2)
        {
            if (s1 == null)
            {
                throw new ArgumentNullException(nameof(s1));
            }

            if (s2 == null)
            {
                throw new ArgumentNullException(nameof(s2));
            }

            if (s1.Equals(s2))
            {
                return 1f;
            }

            var mtp = Matches(s1, s2);
            float m = mtp[0];
            if (m == 0)
            {
                return 0f;
            }
            double j = ((m / s1.Length + m / s2.Length + (m - mtp[1]) / m))
                    / THREE;
            var jw = j;

            if (j > Threshold)
            {
                jw = j + Math.Min(JW_COEF, 1.0 / mtp[THREE]) * mtp[2] * (1 - j);
            }
            return jw;
        }

        /// <summary>
        /// Return 1 - similarity.
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>1 - similarity</returns>
        /// <exception cref="ArgumentNullException">If s1 or s2 is null.</exception>
        public double Distance(string s1, string s2)
            => 1.0 - Similarity(s1, s2);

        private static int[] Matches(string s1, string s2)
        {
            string max, min;
            if (s1.Length > s2.Length)
            {
                max = s1;
                min = s2;
            }
            else
            {
                max = s2;
                min = s1;
            }
            var range = Math.Max((max.Length / 2) - 1, 0);

            //int[] matchIndexes = new int[min.Length];
            //Arrays.fill(matchIndexes, -1);
            var match_indexes = Enumerable.Repeat(-1, min.Length).ToArray();

            var match_flags = new bool[max.Length];
            var matches = 0;
            for (var mi = 0; mi < min.Length; mi++)
            {
                var c1 = min[mi];
                for (int xi = Math.Max(mi - range, 0),
                        xn = Math.Min(mi + range + 1, max.Length); xi < xn; xi++)
                {
                    if (!match_flags[xi] && c1 == max[xi])
                    {
                        match_indexes[mi] = xi;
                        match_flags[xi] = true;
                        matches++;
                        break;
                    }
                }
            }
            var ms1 = new char[matches];
            var ms2 = new char[matches];
            for (int i = 0, si = 0; i < min.Length; i++)
            {
                if (match_indexes[i] != -1)
                {
                    ms1[si] = min[i];
                    si++;
                }
            }
            for (int i = 0, si = 0; i < max.Length; i++)
            {
                if (match_flags[i])
                {
                    ms2[si] = max[i];
                    si++;
                }
            }
            var transpositions = 0;
            for (var mi = 0; mi < ms1.Length; mi++)
            {
                if (ms1[mi] != ms2[mi])
                {
                    transpositions++;
                }
            }
            var prefix = 0;
            for (var mi = 0; mi < min.Length; mi++)
            {
                if (s1[mi] == s2[mi])
                {
                    prefix++;
                }
                else
                {
                    break;
                }
            }
            return new[] { matches, transpositions / 2, prefix, max.Length };
        }
    }
}
