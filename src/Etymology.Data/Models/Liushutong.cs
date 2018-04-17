using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etymology.Data.Models
{
    public partial class Liushutong
    {
        [Required]
        [StringLength(20)]
        public string Traditional { get; set; }
        public int LiushutongId { get; set; }
        public int? SealId { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageVector { get; set; }
        public string ImageVectorBase64 { get; set; }

        [ForeignKey("SealId")]
        [InverseProperty("Liushutong")]
        public Seal Seal { get; set; }
    }
}
