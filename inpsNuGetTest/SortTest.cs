using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    public class SortTest
    {
        #region Standard Sorting Algorithms Tests

        [Fact] public void BubbleSort_SortsCorrectly() => VerifySortAlgorithm(Sort.BubbleSort);
        [Fact] public void CocktailShakerSort_SortsCorrectly() => VerifySortAlgorithm(Sort.CocktailShakerSort);
        [Fact] public void OddEvenSort_SortsCorrectly() => VerifySortAlgorithm(Sort.OddEvenSort);
        [Fact] public void SelectionSort_SortsCorrectly() => VerifySortAlgorithm(Sort.SelectionSort);
        [Fact] public void InsertionSort_SortsCorrectly() => VerifySortAlgorithm(Sort.InsertionSort);
        [Fact] public void ShellSort_SortsCorrectly() => VerifySortAlgorithm(Sort.ShellSort);
        [Fact] public void QuickSort_SortsCorrectly() => VerifySortAlgorithm(Sort.QuickSort);
        [Fact] public void MergeSort_SortsCorrectly() => VerifySortAlgorithm(Sort.MergeSort);
        [Fact] public void HeapSort_SortsCorrectly() => VerifySortAlgorithm(Sort.HeapSort);
        [Fact] public void IntroSort_SortsCorrectly() => VerifySortAlgorithm(Sort.IntroSort);
        [Fact] public void TimSort_SortsCorrectly() => VerifySortAlgorithm(Sort.TimSort);
        [Fact] public void PigeonholeSort_SortsCorrectly() => VerifySortAlgorithm(Sort.PigeonholeSort);
        [Fact] public void TreeSort_SortsCorrectly() => VerifySortAlgorithm(Sort.TreeSort);
        [Fact] public void PatienceSorting_SortsCorrectly() => VerifySortAlgorithm(Sort.PatienceSorting);

        #endregion

        #region Non-Negative Constraint Sorting Algorithms

        [Fact]
        public void CountingSort_SortsCorrectly()
        {
            // CountingSort cannot handle negative numbers because it allocates array sizes based on maxVal.
            VerifySortAlgorithm(Sort.CountingSort, allowNegative: false);
        }

        [Fact]
        public void BeadSort_SortsCorrectly()
        {
            // BeadSort is only for non-negative integers.
            VerifySortAlgorithm(Sort.BeadSort, allowNegative: false);
        }

        [Fact]
        public void BeadSort_NegativeValue_ThrowsArgumentException()
        {
            // Assert that BeadSort throws an exception when encountering a negative value.
            Assert.Throws<ArgumentException>(() => Sort.BeadSort(new[] { 3, -1, 4 }));
        }

        #endregion

        #region BogoSort (Randomized Sort)

        [Fact]
        public void BogoSort_SmallArray_SortsCorrectly()
        {
            // BogoSort has O(N!) average complexity. Testing with 3 or fewer elements avoids long test times.
            Assert.Equal(new[] { 1, 2, 3 }, Sort.BogoSort(new[] { 3, 1, 2 }));
            Assert.Equal(new[] { 5 }, Sort.BogoSort(new[] { 5 }));
            Assert.Empty(Sort.BogoSort(Array.Empty<int>()));
        }

        #endregion

        #region BucketSort (Double Data Type)

        [Fact]
        public void BucketSortUniform_SortsCorrectly()
        {
            // BucketSortUniform maps doubles, typically designed for values in the range [0, 1).
            double[] unsorted = { 0.5, 0.1, 0.9, 0.4, 0.3, 0.8, 0.2 };
            double[] expected = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.8, 0.9 };

            Assert.Equal(expected, Sort.BucketSortUniform(unsorted));
            Assert.Empty(Sort.BucketSortUniform(Array.Empty<double>()));
        }

        #endregion

        #region Generic Helper Method

        /// <summary>
        /// Shared helper to ensure standard array states sort cleanly across different algorithms.
        /// </summary>
        private void VerifySortAlgorithm(Func<int[], int[]> sortFunc, bool allowNegative = true)
        {
            // 1. Standard Case
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, sortFunc(new[] { 5, 3, 4, 1, 2 }));

            // 2. Already Sorted
            Assert.Equal(new[] { 10, 20, 30 }, sortFunc(new[] { 10, 20, 30 }));

            // 3. Reversed Array (Changed to positive numbers)
            Assert.Equal(new[] { 1, 2, 3 }, sortFunc(new[] { 3, 2, 1 }));

            // 4. Duplicates
            Assert.Equal(new[] { 2, 2, 3, 3, 5 }, sortFunc(new[] { 3, 2, 5, 2, 3 }));

            // 5. Single Element
            Assert.Equal(new[] { 42 }, sortFunc(new[] { 42 }));

            // 6. Empty Array
            Assert.Empty(sortFunc(Array.Empty<int>()));

            // 7. Negative Numbers (Only evaluated if algorithm supports it)
            if (allowNegative)
            {
                Assert.Equal(new[] { -20, -10, 0, 10 }, sortFunc(new[] { 10, -10, 0, -20 }));
            }
        }

        #endregion
    }
}
