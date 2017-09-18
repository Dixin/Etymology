namespace Etymology.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Seal
    {
        public Seal()
        {
            Liushutong = new HashSet<Liushutong>();
        }

        [StringLength(255)]
        public string Character { get; set; }
        [StringLength(255)]
        public string Common { get; set; }
        [StringLength(255)]
        public string Rank { get; set; }
        [StringLength(255)]
        public string KaiDecomposition { get; set; }
        [StringLength(255)]
        public string SealDecomposition { get; set; }
        [StringLength(255)]
        public string TransitionMechanism { get; set; }
        [StringLength(255)]
        public string Mandarin { get; set; }
        [StringLength(255)]
        public string ShuowenTraditional { get; set; }
        [Column("BH")]
        [StringLength(255)]
        public string Bh { get; set; }
        [StringLength(255)]
        public string ZhW { get; set; }
        [Column("HXKZ")]
        [StringLength(255)]
        public string Hxkz { get; set; }
        [StringLength(255)]
        public string ShuowenSimplified { get; set; }
        [Column("DX")]
        [StringLength(255)]
        public string Dx { get; set; }
        [Column("DXFQ")]
        [StringLength(255)]
        public string Dxfq { get; set; }
        public int SealId { get; set; }
        [Column(TypeName = "image")]
        public byte[] Image { get; set; }

        [InverseProperty("Seal")]
        public ICollection<Liushutong> Liushutong { get; set; }
    }
}
