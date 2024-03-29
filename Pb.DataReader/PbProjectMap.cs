using CsvHelper.Configuration;
using Pb.DataModel;
// ReSharper disable VirtualMemberCallInConstructor

namespace Pb.DataReader
{
    /// <summary>
    /// Internal class, necessary for the csv parser
    /// </summary>
    internal class PbProjectMap : ClassMap<PbProject>
    {
        public PbProjectMap()
        {
            Map(m => m.Id).Name("project_id");
            Map(m => m.Cost).Name("cost");
            Map(m => m.Name).Name("name");
        }
    }
}
