using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etymology.Data.Models
{
    public partial class Oracle
    {
        [Key]
        public int OracleId { get; set; }
        [Required]
        [StringLength(20)]
        public string Traditional { get; set; }
        public int Rank { get; set; }
        [StringLength(255)]
        public string Combo { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageVector { get; set; }
        public string ImageVectorBase64 { get; set; }
    }
}
