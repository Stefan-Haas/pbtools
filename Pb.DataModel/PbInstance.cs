using System.Collections.Generic;

namespace Pb.DataModel
{
    /// <summary>
    /// Data representation of a file in PABULIB file format
    /// </summary>
    public class PbInstance
    {
        public PbInstance(PbMeta meta, IList<PbProject> projects, IList<PbVoter> votes)
        {
            Meta = meta;
            Projects = projects;
            Votes = votes;
        }

        public PbMeta Meta { get; }
        public IList<PbProject> Projects { get; }
        public IList<PbVoter> Votes { get; }
    }
}
