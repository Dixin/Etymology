namespace Etymology.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Liushutong
    {
        [Required]
        [StringLength(225)]
        public string Character { get; set; }
        public int LiushutongId { get; set; }
        [Column(TypeName = "image")]
        public byte[] Image { get; set; }
        public int? SealId { get; set; }

        [ForeignKey("SealId")]
        [InverseProperty("Liushutong")]
        public Seal Seal { get; set; }
    }
}
