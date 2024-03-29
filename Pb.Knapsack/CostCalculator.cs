using System.Collections.Generic;
using System.Linq;

namespace Pb.Knapsack
{
    /// <summary>
    /// Helper class for frequently occuring calculations of sets of knapsack lists
    /// </summary>
    public static class CostCalculator
    {
        /// <summary>
        /// Return the knapsack with the biggest sum of cost of all items, given a list of knapsacks
        /// </summary>
        public static IReadOnlyList<KnapsackItem> GetSetWithMaximumCost(IReadOnlyList<IReadOnlyList<KnapsackItem>> projectSets)
        {
            long maximumWeight = 0;
            IReadOnlyList<KnapsackItem> maxSet = null;
            foreach (var set in projectSets)
            {
                var weight = GetWeight(set);
                if (weight > maximumWeight)
                {
                    maximumWeight = weight;
                    maxSet = set;
                }
            }

            return maxSet ?? new List<KnapsackItem>();
        }

        /// <summary>
        /// Return the knapsack with the biggest sum of profit of all items, given a list of knapsacks
        /// </summary>
        public static IReadOnlyList<KnapsackItem> GetSetWithMaximumProfit(IReadOnlyList<IReadOnlyList<KnapsackItem>> projectSets)
        {
            long maximumProfit = 0;
            IReadOnlyList<KnapsackItem> maxSet = null;
            foreach (var set in projectSets)
            {
                var profit = GetProfit(set);
                if (profit > maximumProfit)
                {
                    maximumProfit = profit;
                    maxSet = set;
                }
            }

            return maxSet ?? new List<KnapsackItem>();
        }

        /// <summary>
        /// Return the weight of profit of all items, given a knapsack
        /// </summary>
        public static long GetWeight(IReadOnlyList<KnapsackItem> items)
        {
            return items.Sum(project => project.Weight);
        }

        /// <summary>
        /// Return the sum of profit of all items, given a knapsack
        /// </summary>
        public static long GetProfit(IReadOnlyList<KnapsackItem> items)
        {
            return items.Sum(project => project.Profit);
        }
    }
}
