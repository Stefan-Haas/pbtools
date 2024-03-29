using System.Linq;
using CsvHelper.Configuration;
using Pb.DataModel;
// ReSharper disable VirtualMemberCallInConstructor

namespace Pb.DataReader
{
    /// <summary>
    /// Internal class, necessary for the csv parser
    /// </summary>
    internal class PbVoterMap : ClassMap<PbVoter>
    {
        public PbVoterMap()
        {
            Map(m => m.Id).Name("voter_id");
            Map(m => m.Vote).Name("vote").Convert(row =>
            {
                var columnValue = row.Row.GetField("vote");
                var list = columnValue?.Split(',').ToList();
                return list;
            });
        }
    }
}
