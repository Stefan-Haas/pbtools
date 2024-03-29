namespace Pb.DataModel
{
    /// <summary>
    /// Data representation of the Meta section from PABULIB file format
    /// </summary>
    public class PbMeta
    {
        public string FileName { get; set; }
        public double Budget { get; set; }
        public VoteType VoteType { get; set; }
    }
}
