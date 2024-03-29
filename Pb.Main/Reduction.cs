using System.Collections.Generic;
using Pb.DataModel;
using Pb.Knapsack;

namespace Pb.Main
{
    public static class Reduction
    {
        /// <summary>
        /// Performs the transformation from PB to Knapsack
        /// </summary>
        public static List<KnapsackItem> MakeReductionToKnapsack(PbInstance pbInstance)
        {
            var knapsack = new List<KnapsackItem>();

            foreach (var project in pbInstance.Projects)
            {
                knapsack.Add(new KnapsackItem(project.Id, project.Cost, 0));
            }

            foreach (var pbVoter in pbInstance.Votes)
            {
                foreach (var vote in pbVoter.Vote)
                {
                    var item = knapsack.Find(i => i.Id.Equals(vote));
                    if (item != null)
                    {
                        item.Profit += item.Weight;
                    }
                }
            }

            return knapsack;
        }
    }
}
