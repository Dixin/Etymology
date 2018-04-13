namespace Etymology.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Bronze
    {
        [Required]
        [StringLength(255)]
        public string Character { get; set; }
        public int? Rank { get; set; }
        public int BronzeId { get; set; }
        public string ImageBase64 { get; set; }
    }
}
