namespace Pb.DataModel
{
    /// <summary>
    /// Data representation of the Project section from PABULIB file format
    /// </summary>
    public class PbProject
    {
        public string Id { get; set; }
        public int Cost { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}
