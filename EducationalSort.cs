namespace inpsNuGet;

/// <summary>
/// Algorithms that are mainly for learning or fun. Like <see cref="Sort"/>, every method returns a NEW array.
/// Don't use these on production workloads.
/// </summary>
public static class EducationalSort
{
    const int MaxBeadColumns = 1 << 20;

    /// <summary>Randomly shuffles until sorted (expected O(n * n!)). Use <paramref name="cancellationToken"/> to bail out.</summary>
    public static int[] BogoSort(int[] arr, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arr);
        int[] a = (int[])arr.Clone();

        while (!IsSorted(a))
        {
            cancellationToken.ThrowIfCancellationRequested();
            for (int i = a.Length - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (a[i], a[j]) = (a[j], a[i]);
            }
        }
        return a;
    }

    static bool IsSorted(int[] a)
    {
        for (int i = 1; i < a.Length; i++)
            if (a[i - 1] > a[i]) return false;
        return true;
    }

    /// <summary>Gravity sort for non-negative integers. O(n * max) time; max is capped at 2^20.</summary>
    public static int[] BeadSort(int[] arr)
    {
        ArgumentNullException.ThrowIfNull(arr);
        if (arr.Length == 0) return [];
        if (arr.Any(x => x < 0)) throw new ArgumentException("Bead sort supports non-negative integers only.", nameof(arr));

        int max = arr.Max();
        if (max > MaxBeadColumns)
            throw new ArgumentException($"Largest value exceeds the supported maximum ({MaxBeadColumns:N0}).", nameof(arr));

        int n = arr.Length;
        var columnBeads = new int[max];
        foreach (int x in arr)
            for (int j = 0; j < x; j++) columnBeads[j]++;

        var result = new int[n];
        for (int i = 0; i < n; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < max; j++)
                if (columnBeads[j] >= n - i) rowSum++;
            result[i] = rowSum;
        }
        return result;
    }

    /// <summary>
    /// Binary-search-tree sort. Iterative (no stack overflow), but the tree is unbalanced,
    /// so already-sorted input degrades to O(n^2).
    /// </summary>
    public static int[] TreeSort(int[] arr)
    {
        ArgumentNullException.ThrowIfNull(arr);
        TreeNode? root = null;

        foreach (int x in arr)
        {
            var node = new TreeNode(x);
            if (root is null) { root = node; continue; }

            TreeNode cur = root;
            while (true)
            {
                if (x < cur.Value)
                {
                    if (cur.Left is null) { cur.Left = node; break; }
                    cur = cur.Left;
                }
                else
                {
                    if (cur.Right is null) { cur.Right = node; break; }
                    cur = cur.Right;
                }
            }
        }

        var result = new int[arr.Length];
        int pos = 0;
        var stack = new Stack<TreeNode>();
        TreeNode? current = root;
        while (current is not null || stack.Count > 0)
        {
            while (current is not null) { stack.Push(current); current = current.Left; }
            current = stack.Pop();
            result[pos++] = current.Value;
            current = current.Right;
        }
        return result;
    }

    sealed class TreeNode(int value)
    {
        public readonly int Value = value;
        public TreeNode? Left, Right;
    }
}
