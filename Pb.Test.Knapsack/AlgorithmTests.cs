using System.Linq;
using NUnit.Framework;
using Pb.Knapsack;

namespace Pb.Test.Knapsack
{
    [TestFixture]
    internal class AlgorithmTests
    {
        [Test]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(5000)]
        [Repeat(5)]
        public void CalculateKnapsackWithDifferentAlgorithms_NRandomItems_ShouldReturnSameResults(int n)
        {
            var items = DataGenerator.CreateKnapsackItems(n, 0, 100);
            var capacity = CostCalculator.GetWeight(items) / 2;

            var bellmanSolution = Algorithm.Bellman(items.ToList(), capacity);
            var primalDualSolution = Algorithm.PrimalDualBranch(items.ToList(), capacity);

            var wSumBellman = CostCalculator.GetWeight(bellmanSolution);
            var wSumPrimalDual = CostCalculator.GetWeight(primalDualSolution);
            var pSumBellman = CostCalculator.GetProfit(bellmanSolution);
            var pSumPrimalDual = CostCalculator.GetProfit(primalDualSolution);

            Assert.That(wSumBellman, Is.LessThanOrEqualTo(capacity));
            Assert.That(wSumPrimalDual, Is.LessThanOrEqualTo(capacity));

            Assert.That(pSumBellman, Is.EqualTo(pSumPrimalDual));
        }
    }
}
