using System;

namespace Pb.Knapsack
{
    /// <summary>
    /// Data representation of a knapsack item
    /// </summary>
    public class KnapsackItem : IComparable
    {
        public KnapsackItem(string id, long weight, long profit)
        {
            Id = id;
            Weight = weight;
            Profit = profit;
        }

        public string Id { get; }
        public long Weight { get; }
        public long Profit { get; set; }

        public double Efficiency => (double)Profit / Weight;

        public override string ToString()
        {
            return $"Id: {Id}, Profit: {Profit}, Weight: {Weight}, Efficiency: {Efficiency}";
        }

        public int CompareTo(object other)
        {
            return other is not KnapsackItem item ? 1 : Efficiency.CompareTo(item.Efficiency);
        }

        public static bool operator <(KnapsackItem a, KnapsackItem b) => a.Efficiency < b.Efficiency;

        public static bool operator <=(KnapsackItem a, KnapsackItem b) => a.Efficiency <= b.Efficiency;

        public static bool operator >(KnapsackItem a, KnapsackItem b) => a.Efficiency > b.Efficiency;

        public static bool operator >=(KnapsackItem a, KnapsackItem b) => a.Efficiency >= b.Efficiency;
    }
}
