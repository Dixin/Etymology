using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etymology.Data.Models
{
    public partial class Bronze
    {
        [Required]
        [StringLength(20)]
        public string Traditional { get; set; }
        public int? Rank { get; set; }
        public string ImageBase64 { get; set; }
        public string ImageVector { get; set; }
        [Key]
        public int BronzeId { get; set; }
        public string ImageVectorBase64 { get; set; }
    }
}
