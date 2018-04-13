namespace Etymology.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Oracle
    {
        [StringLength(255)]
        public string Character { get; set; }
        public double? Rank { get; set; }
        [StringLength(255)]
        public string Combo { get; set; }
        public int OracleId { get; set; }
        public string ImageBase64 { get; set; }
    }
}
