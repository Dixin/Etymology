using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etymology.Data.Models
{
    public partial class Seal
    {
        public Seal()
        {
            Liushutong = new HashSet<Liushutong>();
        }

        public int SealId { get; set; }
        [Required]
        [StringLength(20)]
        public string Traditional { get; set; }
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
        public string ImageBase64 { get; set; }
        public string ImageVector { get; set; }
        public string ImageVectorBase64 { get; set; }

        [InverseProperty("Seal")]
        public ICollection<Liushutong> Liushutong { get; set; }
    }
}
