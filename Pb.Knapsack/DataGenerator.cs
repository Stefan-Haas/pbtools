using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;

namespace Pb.Knapsack
{
    /// <summary>
    /// Helper class for data generation of knapsack items with uniform distribution of data in a given range
    /// </summary>
    public static class DataGenerator
    {
        private static readonly Random _rand = new Mrg32k3a(RandomSeed.Robust());

        /// <summary>
        /// Returns a knapsack, where the weight and profit of items is uniformly distributed
        /// </summary>
        /// <param name="count">Number of items</param>
        /// <param name="lowerBound">Lower bound of weight and profit</param>
        /// <param name="upperBound">Upper bound of weight and profit</param>
        public static List<KnapsackItem> CreateKnapsackItems(int count, int lowerBound, int upperBound)
        {
            var result = new List<KnapsackItem>();

            for (int i = 0; i < count; i++)
            {
                var weight = _rand.Next(lowerBound, upperBound + 1);
                var profit = _rand.Next(lowerBound, upperBound + 1);
                var item = new KnapsackItem(i.ToString(), weight, profit);
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Returns a knapsack, where the weight and profit of items is uniformly distributed, weights and profits are correlated
        /// </summary>
        /// <param name="count">Number of items</param>
        /// <param name="lowerBound">Lower bound of weight and profit</param>
        /// <param name="upperBound">Upper bound of weight and profit</param>
        /// <param name="correlation">Pearson correlation between weights and profits</param>
        /// <returns></returns>
        public static List<KnapsackItem> CreateKnapsackItemsWithCorrelation(int count, int lowerBound, int upperBound, double correlation)
        {
            var result = new List<KnapsackItem>();

            for (int i = 0; i < count; i++)
            {
                var weight = _rand.Next(lowerBound, upperBound + 1);
                var tmp = _rand.Next(lowerBound, upperBound + 1);
                var profit = (int)(correlation * weight + Math.Sqrt(1 - correlation * correlation) * tmp);
                var item = new KnapsackItem(i.ToString(), weight, profit);
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Returns a knapsack, where the weight and profit of items is uniformly distributed. Profits are multiplied by a value between 0 and weightFactor with weight
        /// </summary>
        /// <param name="count">Number of items</param>
        /// <param name="lowerBound">Lower bound of weight</param>
        /// <param name="upperBound">Upper bound of weight</param>
        /// <param name="weightFactor">Upper bound of profit multiplier</param>
        public static List<KnapsackItem> CreateKnapsackItems(int count, int lowerBound, int upperBound, int weightFactor)
        {
            var result = new List<KnapsackItem>();

            for (int i = 0; i < count; i++)
            {
                var weight = _rand.Next(lowerBound, upperBound + 1);
                var votes = _rand.Next(0, weightFactor + 1);
                var profit = weight * votes;
                var item = new KnapsackItem(i.ToString(), weight, profit);
                result.Add(item);
            }

            return result;
        }
    }
}
