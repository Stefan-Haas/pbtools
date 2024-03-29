using System.Collections.Generic;

namespace Pb.DataModel
{
    /// <summary>
    /// Data representation of the Voter section from PABULIB file format
    /// </summary>
    public class PbVoter
    {
        public string Id { get; set; }
        public List<string> Vote { get; set; }
    }
}
