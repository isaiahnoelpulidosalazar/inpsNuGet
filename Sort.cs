using System.Numerics;

namespace inpsNuGet;

/// <summary>
/// Sorting algorithms for <c>int[]</c>. Every method returns a NEW sorted array and never modifies its input.
/// For intentionally impractical algorithms see <see cref="EducationalSort"/>.
/// </summary>
public static class Sort
{
    const int MaxCountingRange = 1 << 24; // caps counting-sort memory at ~64 MB
    const int InsertionThreshold = 16;

    static void Swap(int[] a, int i, int j) => (a[i], a[j]) = (a[j], a[i]);

    static int[] Copy(int[] a)
    {
        ArgumentNullException.ThrowIfNull(a);
        return (int[])a.Clone();
    }

    // ---- Simple O(n^2) sorts -------------------------------------------------

    public static int[] BubbleSort(int[] arr)
    {
        int[] a = Copy(arr);
        for (int i = 0; i < a.Length - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < a.Length - i - 1; j++)
                if (a[j] > a[j + 1]) { Swap(a, j, j + 1); swapped = true; }
            if (!swapped) break;
        }
        return a;
    }

    public static int[] CocktailShakerSort(int[] arr)
    {
        int[] a = Copy(arr);
        int start = 0, end = a.Length - 1;
        bool swapped = true;
        while (swapped && start < end)
        {
            swapped = false;
            for (int i = start; i < end; i++)
                if (a[i] > a[i + 1]) { Swap(a, i, i + 1); swapped = true; }
            end--;
            if (!swapped) break;

            swapped = false;
            for (int i = end - 1; i >= start; i--)
                if (a[i] > a[i + 1]) { Swap(a, i, i + 1); swapped = true; }
            start++;
        }
        return a;
    }

    public static int[] OddEvenSort(int[] arr)
    {
        int[] a = Copy(arr);
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = 1; i < a.Length - 1; i += 2)
                if (a[i] > a[i + 1]) { Swap(a, i, i + 1); sorted = false; }
            for (int i = 0; i < a.Length - 1; i += 2)
                if (a[i] > a[i + 1]) { Swap(a, i, i + 1); sorted = false; }
        }
        return a;
    }

    public static int[] SelectionSort(int[] arr)
    {
        int[] a = Copy(arr);
        for (int i = 0; i < a.Length - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < a.Length; j++)
                if (a[j] < a[min]) min = j;
            if (min != i) Swap(a, min, i);
        }
        return a;
    }

    public static int[] InsertionSort(int[] arr)
    {
        int[] a = Copy(arr);
        InsertionRange(a, 0, a.Length - 1);
        return a;
    }

    static void InsertionRange(int[] a, int lo, int hi)
    {
        for (int i = lo + 1; i <= hi; i++)
        {
            int key = a[i], j = i - 1;
            while (j >= lo && a[j] > key) { a[j + 1] = a[j]; j--; }
            a[j + 1] = key;
        }
    }

    public static int[] ShellSort(int[] arr)
    {
        int[] a = Copy(arr);
        for (int gap = a.Length / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < a.Length; i++)
            {
                int temp = a[i], j = i;
                while (j >= gap && a[j - gap] > temp) { a[j] = a[j - gap]; j -= gap; }
                a[j] = temp;
            }
        }
        return a;
    }

    // ---- O(n log n) sorts ----------------------------------------------------

    /// <summary>In-place 3-way quicksort (median-of-three pivot); fast on inputs with many duplicates.</summary>
    public static int[] QuickSort(int[] arr)
    {
        int[] a = Copy(arr);
        QuickRange(a, 0, a.Length - 1);
        return a;
    }

    static void QuickRange(int[] a, int lo, int hi)
    {
        while (lo < hi)
        {
            int pivot = MedianOfThree(a[lo], a[lo + (hi - lo) / 2], a[hi]);
            int lt = lo, i = lo, gt = hi;
            while (i <= gt)
            {
                if (a[i] < pivot) Swap(a, lt++, i++);
                else if (a[i] > pivot) Swap(a, i, gt--);
                else i++;
            }
            // Recurse into the smaller side, loop on the larger: stack depth stays O(log n).
            if (lt - lo < hi - gt) { QuickRange(a, lo, lt - 1); lo = gt + 1; }
            else { QuickRange(a, gt + 1, hi); hi = lt - 1; }
        }
    }

    static int MedianOfThree(int x, int y, int z) =>
        x < y ? (y < z ? y : (x < z ? z : x))
              : (x < z ? x : (y < z ? z : y));

    /// <summary>Stable top-down merge sort using a single shared buffer.</summary>
    public static int[] MergeSort(int[] arr)
    {
        int[] a = Copy(arr);
        if (a.Length > 1) MergeRange(a, new int[a.Length], 0, a.Length);
        return a;
    }

    static void MergeRange(int[] a, int[] buf, int lo, int hi) // [lo, hi)
    {
        if (hi - lo < 2) return;
        int mid = lo + (hi - lo) / 2;
        MergeRange(a, buf, lo, mid);
        MergeRange(a, buf, mid, hi);
        if (a[mid - 1] <= a[mid]) return;

        int i = lo, j = mid, k = lo;
        while (i < mid && j < hi) buf[k++] = a[i] <= a[j] ? a[i++] : a[j++];
        while (i < mid) buf[k++] = a[i++];
        while (j < hi) buf[k++] = a[j++];
        Array.Copy(buf, lo, a, lo, hi - lo);
    }

    /// <summary>In-place heap sort (on a copy).</summary>
    public static int[] HeapSort(int[] arr)
    {
        int[] a = Copy(arr);
        HeapRange(a, 0, a.Length - 1);
        return a;
    }

    static void HeapRange(int[] a, int lo, int hi)
    {
        int n = hi - lo + 1;
        for (int i = n / 2 - 1; i >= 0; i--) SiftDown(a, lo, i, n);
        for (int end = n - 1; end > 0; end--)
        {
            Swap(a, lo, lo + end);
            SiftDown(a, lo, 0, end);
        }
    }

    static void SiftDown(int[] a, int lo, int root, int n)
    {
        while (true)
        {
            int child = 2 * root + 1;
            if (child >= n) return;
            if (child + 1 < n && a[lo + child + 1] > a[lo + child]) child++;
            if (a[lo + root] >= a[lo + child]) return;
            Swap(a, lo + root, lo + child);
            root = child;
        }
    }

    /// <summary>Quicksort that falls back to heap sort past 2*log2(n) depth and to insertion sort on small ranges.</summary>
    public static int[] IntroSort(int[] arr)
    {
        int[] a = Copy(arr);
        if (a.Length > 1) IntroRange(a, 0, a.Length - 1, 2 * BitOperations.Log2((uint)a.Length));
        return a;
    }

    static void IntroRange(int[] a, int lo, int hi, int depth)
    {
        while (hi - lo > InsertionThreshold)
        {
            if (depth-- == 0) { HeapRange(a, lo, hi); return; }
            int p = Partition(a, lo, hi);
            if (p - lo < hi - p) { IntroRange(a, lo, p - 1, depth); lo = p + 1; }
            else { IntroRange(a, p + 1, hi, depth); hi = p - 1; }
        }
        InsertionRange(a, lo, hi);
    }

    static int Partition(int[] a, int lo, int hi)
    {
        int mid = lo + (hi - lo) / 2;
        if (a[mid] < a[lo]) Swap(a, mid, lo);
        if (a[hi] < a[lo]) Swap(a, hi, lo);
        if (a[hi] < a[mid]) Swap(a, hi, mid);
        Swap(a, mid, hi); // pivot (median) to the end

        int pivot = a[hi], i = lo;
        for (int j = lo; j < hi; j++)
            if (a[j] < pivot) Swap(a, i++, j);
        Swap(a, i, hi);
        return i;
    }

    /// <summary>The framework's <see cref="Array.Sort(Array)"/> (introsort) on a copy. Not Tim sort.</summary>
    public static int[] FrameworkSort(int[] arr)
    {
        int[] a = Copy(arr);
        Array.Sort(a);
        return a;
    }

    // ---- Non-comparison sorts ------------------------------------------------

    /// <summary>Counting sort. Negative values are supported; throws if (max - min + 1) exceeds 2^24.</summary>
    public static int[] CountingSort(int[] arr)
    {
        ArgumentNullException.ThrowIfNull(arr);
        if (arr.Length == 0) return [];

        int min = arr.Min(), max = arr.Max();
        long range = (long)max - min + 1;
        if (range > MaxCountingRange)
            throw new ArgumentException(
                $"Value range ({range:N0}) exceeds the supported maximum ({MaxCountingRange:N0}).", nameof(arr));

        var counts = new int[range];
        foreach (int x in arr) counts[(long)x - min]++;

        var result = new int[arr.Length];
        int pos = 0;
        for (int v = 0; v < counts.Length; v++)
        {
            result.AsSpan(pos, counts[v]).Fill(v + min);
            pos += counts[v];
        }
        return result;
    }

    /// <summary>Bucket sort for values in [0.0, 1.0). Throws <see cref="ArgumentOutOfRangeException"/> otherwise (incl. NaN).</summary>
    public static double[] BucketSortUniform(double[] arr)
    {
        ArgumentNullException.ThrowIfNull(arr);
        int n = arr.Length;
        var result = new double[n];
        if (n == 0) return result;

        var buckets = new List<double>?[n];
        foreach (double x in arr)
        {
            if (!(x >= 0.0 && x < 1.0))
                throw new ArgumentOutOfRangeException(nameof(arr), "Values must be in [0.0, 1.0).");
            int index = Math.Min((int)(x * n), n - 1);
            (buckets[index] ??= new List<double>()).Add(x);
        }

        int pos = 0;
        foreach (var b in buckets)
        {
            if (b is null) continue;
            b.Sort();
            b.CopyTo(result, pos);
            pos += b.Count;
        }
        return result;
    }

    /// <summary>Patience sort: deal into piles with binary search, then k-way merge the piles.</summary>
    public static int[] PatienceSort(int[] arr)
    {
        ArgumentNullException.ThrowIfNull(arr);
        var piles = new List<List<int>>();

        foreach (int x in arr)
        {
            int left = 0, right = piles.Count;
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (piles[mid][^1] >= x) right = mid;
                else left = mid + 1;
            }
            if (left < piles.Count) piles[left].Add(x);
            else piles.Add([x]);
        }

        var pq = new PriorityQueue<(int Pile, int Item), int>();
        for (int i = 0; i < piles.Count; i++)
            pq.Enqueue((i, piles[i].Count - 1), piles[i][^1]);

        var result = new int[arr.Length];
        int idx = 0;
        while (pq.TryDequeue(out var cur, out int val))
        {
            result[idx++] = val;
            int next = cur.Item - 1;
            if (next >= 0) pq.Enqueue((cur.Pile, next), piles[cur.Pile][next]);
        }
        return result;
    }
}
