using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Pb.Knapsack
{
    /// <summary>
    /// Collection of algorithms for solving the knapsack problem
    /// </summary>
    public static class Algorithm
    {
        /// <summary>
        /// Solves the knapsack problem with a dynamic programming approach, known as Bellman algorithm
        /// </summary>
        public static List<KnapsackItem> Bellman(List<KnapsackItem> items, long capacity, CancellationToken ct = default)
        {
            long[,] z = new long[items.Count + 1, capacity + 1];
            long[,] a = new long[items.Count + 1, capacity + 1];

            for (int d = 0; d <= capacity; d++)
            {
                z[0, d] = 0;
            }

            for (int j = 1; j <= items.Count; j++)
            {
                long weight = items[j - 1].Weight;
                long profit = items[j - 1].Profit;

                for (int d = 0; d < weight; d++)
                {
                    z[j, d] = z[j - 1, d];
                    a[j, d] = 0;
                }

                for (var d = weight; d <= capacity; d++)
                {
                    if (z[j - 1, d - weight] + profit > z[j - 1, d])
                    {
                        z[j, d] = z[j - 1, d - weight] + profit;
                        a[j, d] = 1;
                    }
                    else
                    {
                        z[j, d] = z[j - 1, d];
                        a[j, d] = 0;
                    }

                    ct.ThrowIfCancellationRequested();
                }
            }

            return Backtrack(a, items.Count + 1, capacity + 1, items);
        }

        /// <summary>
        /// Backtracks the table calculated from Bellman to return a solution.
        /// </summary>
        private static List<KnapsackItem> Backtrack(long[,] array, int row, long column, IReadOnlyList<KnapsackItem> items)
        {
            var result = new List<KnapsackItem>();

            int n = row;
            long c = column;

            while (n > 0)
            {
                if (array[n - 1, c - 1] == 1)
                {
                    result.Add(items[n - 2]);
                    n--;
                    c -= items[n - 1].Weight;
                }
                else
                {
                    n--;
                }
            }

            return result;
        }

        /// <summary>
        /// Solves the knapsack problem with a branch and bound approach, known as Primal-Branch
        /// </summary>
        public static List<KnapsackItem> PrimalBranch(List<KnapsackItem> items, long capacity, CancellationToken ct = default)
        {
            items.Sort((a, b) => b.Efficiency.CompareTo(a.Efficiency));

            var minWeights = new long[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                var tmp = items.Skip(i);
                minWeights[i] = tmp.Min(t => t.Weight);
            }

            var solution = new int[items.Count];
            long lowerBound = 0;
            var bStar = -1;
            PrimalBranchImpl(0, 0, 0, capacity, items.ToArray(), ref lowerBound, ref bStar, solution, minWeights.ToArray(), ct);

            var result = new List<KnapsackItem>();
            for (int i = 0; i < items.Count && i < bStar; i++)
            {
                if (solution[i] == 1)
                {
                    result.Add(items[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Implementation of PrimalBranch
        /// </summary>
        private static bool PrimalBranchImpl(int b, long profitSum, long weightSum, long capacity, KnapsackItem[] items, ref long lowerBound, ref int bStar, int[] x, long[] minWeights, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var improved = false;

            if (weightSum > capacity)
            {
                return false;
            }

            if (profitSum > lowerBound)
            {
                lowerBound = profitSum;
                bStar = b;
                improved = true;
            }

            if (b > items.Length - 1 || ((capacity - weightSum) < minWeights[b]))
            {
                return improved;
            }

            int upperBound = (int)System.Math.Floor((capacity - weightSum) * items[b].Efficiency);
            if (profitSum + upperBound <= lowerBound)
            {
                return false;
            }

            long pb = items[b].Profit;
            long wb = items[b].Weight;
            if (PrimalBranchImpl(b + 1, profitSum + pb, weightSum + wb, capacity, items, ref lowerBound, ref bStar, x, minWeights, ct))
            {
                x[b] = 1;
                improved = true;
            }

            if (PrimalBranchImpl(b + 1, profitSum, weightSum, capacity, items, ref lowerBound, ref bStar, x, minWeights, ct))
            {
                x[b] = 0;
                improved = true;
            }

            return improved;
        }

        /// <summary>
        /// Solves the knapsack problem with a branch and bound approach, known as Primal-Dual-Branch
        /// </summary>
        public static List<KnapsackItem> PrimalDualBranch(List<KnapsackItem> items, long capacity, CancellationToken ct = default)
        {
            items.Sort((a, b) => b.Efficiency.CompareTo(a.Efficiency));

            var tmpCapacity = capacity;
            var splitIdx = -1;
            long splitWeight = 0;
            long splitProfit = 0;
            for (int i = 0; i < items.Count; i++)
            {
                tmpCapacity -= items[i].Weight;
                splitIdx = i;
                if (splitIdx == items.Count - 1 && tmpCapacity >= 0)
                {
                    splitIdx += 1;
                }

                if (tmpCapacity < 0)
                {
                    break;
                }

                splitWeight += items[i].Weight;
                splitProfit += items[i].Profit;
            }

            var solution = new int[items.Count];
            long lowerBound = 0;
            var aStar = -1;
            var bStar = -1;
            PrimalDualBranchImpl(splitIdx - 1, splitIdx, splitProfit, splitWeight, items.ToArray(), capacity, solution, ref lowerBound, ref aStar, ref bStar, ct);

            var result = new List<KnapsackItem>();
            for (int i = 0; i < items.Count && i < bStar; i++)
            {
                if (i < aStar + 1)
                {
                    solution[i] = 1;
                }

                if (solution[i] == 1)
                {
                    result.Add(items[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Implementation of PrimalDualBranch
        /// </summary>
        private static bool PrimalDualBranchImpl(int a, int b, long profitSum, long weightSum, KnapsackItem[] items, long capacity, int[] x, ref long lowerBound, ref int aStar, ref int bStar, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var improved = false;
            if (weightSum <= capacity)
            {
                if (profitSum > lowerBound)
                {
                    lowerBound = profitSum;
                    aStar = a;
                    bStar = b;
                    improved = true;
                }

                if (b > items.Length - 1)
                {
                    return improved;
                }

                long upperBound = (int)System.Math.Floor((capacity - weightSum) * items[b].Efficiency);
                if (profitSum + upperBound <= lowerBound)
                {
                    return improved;
                }

                var pb = items[b].Profit;
                var wb = items[b].Weight;
                if (PrimalDualBranchImpl(a, b + 1, profitSum + pb, weightSum + wb, items, capacity, x, ref lowerBound, ref aStar, ref bStar, ct))
                {
                    x[b] = 1;
                    improved = true;
                }

                if (PrimalDualBranchImpl(a, b + 1, profitSum, weightSum, items, capacity, x, ref lowerBound, ref aStar, ref bStar, ct))
                {
                    x[b] = 0;
                    improved = true;
                }
            }
            else
            {
                if (a < 0)
                {
                    return false;
                }

                long upperBound = (int)System.Math.Floor((capacity - weightSum) * items[a].Efficiency);
                if (profitSum + upperBound <= lowerBound)
                {
                    return false;
                }

                var pa = items[a].Profit;
                var wa = items[a].Weight;
                if (PrimalDualBranchImpl(a - 1, b, profitSum - pa, weightSum - wa, items, capacity, x, ref lowerBound, ref aStar, ref bStar, ct))
                {
                    x[a] = 0;
                    improved = true;
                }

                if (PrimalDualBranchImpl(a - 1, b, profitSum, weightSum, items, capacity, x, ref lowerBound, ref aStar, ref bStar, ct))
                {
                    x[a] = 1;
                    improved = true;
                }
            }

            return improved;
        }

        /// <summary>
        /// Solves the knapsack problem with a simple enumeration of all possible combinations and selecting the knapsack with the maximum profit
        /// </summary>
        public static List<KnapsackItem> Enumeration(List<KnapsackItem> items, long capacity, CancellationToken ct = default)
        {
            // it is possible to use CalculateFeasibleSetRecursiveDirect or CalculateFeasibleSetRecursiveTree
            var solutions = CalculateFeasibleSetRecursiveTree(items.ToList(), capacity, ct);
            var result = CostCalculator.GetSetWithMaximumProfit(solutions).ToList();

            return result;
        }

        /// <summary>
        /// Calculates all possible combinations of feasible knapsacks
        /// </summary>
        private static List<List<KnapsackItem>> CalculateFeasibleSetRecursiveDirect(IList<KnapsackItem> items, long capacity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (!items.Any())
            {
                return new List<List<KnapsackItem>> { new() };
            }

            var elem = items.ElementAt(0);
            items.RemoveAt(0);
            var subsets = CalculateFeasibleSetRecursiveDirect(items, capacity, ct);

            var newSubsets = FilterFeasibleKnapsacks(subsets, elem, capacity, ct);

            subsets.AddRange(newSubsets);
            return subsets;
        }

        /// <summary>
        /// Calculates all possible combinations of feasible knapsacks with a different approach
        /// </summary>
        private static List<List<KnapsackItem>> CalculateFeasibleSetRecursiveTree(IReadOnlyList<KnapsackItem> items, long capacity, CancellationToken ct)
        {
            var start = new List<KnapsackItem>();
            var result = new List<List<KnapsackItem>> { start };
            CalculateFeasibleSetRecursiveTreeImpl(items, start, result, 0, capacity, ct);

            return result;
        }

        /// <summary>
        /// Implementation of CalculateFeasibleSetRecursiveTree
        /// </summary>
        private static void CalculateFeasibleSetRecursiveTreeImpl(IReadOnlyList<KnapsackItem> items, IReadOnlyList<KnapsackItem> currentItems, ICollection<List<KnapsackItem>> result, int idx, long capacity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (idx == items.Count || capacity <= 0)
            {
                return;
            }

            CalculateFeasibleSetRecursiveTreeImpl(items, currentItems, result, idx + 1, capacity, ct);

            if (capacity >= items[idx].Weight)
            {
                var listB = currentItems.ToList();
                listB.Add(items[idx]);
                result.Add(listB);
                CalculateFeasibleSetRecursiveTreeImpl(items, listB, result, idx + 1, capacity - items[idx].Weight, ct);
            }
        }

        /// <summary>
        /// Given a list of knapsacks, this method adds another knapsack item to every knapsack and returns a list of knapsacks that are feasible
        /// </summary>
        private static IEnumerable<List<KnapsackItem>> FilterFeasibleKnapsacks(IEnumerable<IEnumerable<KnapsackItem>> knapsacks, KnapsackItem itemToAdd, long capacity, CancellationToken ct = default)
        {
            var result = new List<List<KnapsackItem>>();
            foreach (var knapsack in knapsacks)
            {
                ct.ThrowIfCancellationRequested();
                var tmp = knapsack.Append(itemToAdd).ToList();
                if (IsFeasible(tmp, capacity))
                {
                    result.Add(tmp);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns if a knapsack is feasible within the capacity given
        /// </summary>
        private static bool IsFeasible(IEnumerable<KnapsackItem> set, long capacity)
        {
            long cost = set.Sum(i => i.Weight);
            return cost <= capacity;
        }
    }
}
