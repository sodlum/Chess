using System;

namespace Chess.Extensions {
    /// <summary>
    /// Chess.Extensions.ExtensionMethods
    /// Provides extension methods for comparable value types.
    /// </summary>
    public static class ExtensionMethods {
        /// <summary>
        /// Determines whether <paramref name="x"/> falls within the range of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <typeparam name="T">The comparable value type.</typeparam>
        /// <param name="x">Value being compared.</param>
        /// <param name="a">Lower boundary.</param>
        /// <param name="b">Upper boundary.</param>
        /// <param name="includeMinBoundary">Optional parameter to include left boundary in comparison.</param>
        /// <param name="includeMaxBoundary">Optional parameter to include right boundary in comparison.</param>
        /// <returns>True if <paramref name="x"/> falls between <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static bool IsBetween<T>(this T x, T a, T b, bool includeRightBoundary = true) where T : struct, IComparable<T> {
            T min = Min(a, b);
            T max = Max(a, b);

            int cmin = min.CompareTo(x);
            int cmax = max.CompareTo(x);

            if (includeRightBoundary) {
                return cmin <= 0 && 0 <= cmax;
            } else {
                return cmin <= 0 && 0 < cmax;
            }
        }

        /// <summary>
        /// Gets the minimum value between <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <typeparam name="T">The comparable value type.</typeparam>
        /// <param name="a">First value for comparison.</param>
        /// <param name="b">Second value for comparison.</param>
        /// <returns>The smaller of the two values.</returns>
        public static T Min<T>(T a, T b) where T : IComparable<T> {
            var comparison = a.CompareTo(b);

            if (comparison <= 0) {
                return a;
            } else {
                return b;
            }
        }

        /// <summary>
        /// Gets the maximum value between <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <typeparam name="T">The comparable value type.</typeparam>
        /// <param name="a">First value for comparison.</param>
        /// <param name="b">Second value for comparison.</param>
        /// <returns>The larger of the two values.</returns>
        public static T Max<T>(T a, T b) where T : IComparable<T> {
            var comparison = a.CompareTo(b);

            if(comparison >= 0) {
                return a;
            } else {
                return b;
            }
        }

        /// <summary>
        /// Gets the total distance moved.
        /// </summary>
        /// <param name="a">First value to compare.</param>
        /// <param name="b">Second value to compare.</param>
        /// <returns>The integer value of the distance between <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static int GetDistance<T>(T a, T b) where T : IComparable<T> {
            T min = Min(a, b);
            T max = Max(a, b);

            return Convert.ToInt32(max) - Convert.ToInt32(min);
        }
    }
}
