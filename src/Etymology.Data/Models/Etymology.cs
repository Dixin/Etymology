using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etymology.Data.Models
{
    public partial class Etymology
    {
        [Required]
        [StringLength(5)]
        public string Simplified { get; set; }
        [Required]
        [StringLength(5)]
        public string Traditional { get; set; }
        [StringLength(10)]
        public string OldTraditional { get; set; }
        [StringLength(15)]
        public string Pinyin { get; set; }
        [StringLength(10)]
        public string Index8105 { get; set; }
        [StringLength(30)]
        public string SimplificationRule { get; set; }
        [StringLength(200)]
        public string SimplificationClarified { get; set; }
        [StringLength(40)]
        public string VariantRule { get; set; }
        [StringLength(255)]
        public string VariantClarified { get; set; }
        [StringLength(30)]
        public string AppliedRule { get; set; }
        [StringLength(10)]
        public string FontRule { get; set; }
        [StringLength(255)]
        public string Decomposition { get; set; }
        [StringLength(255)]
        public string DecompositionClarified { get; set; }
        [StringLength(255)]
        public string OriginalMeaning { get; set; }
        [StringLength(50)]
        public string Videos { get; set; }
        [StringLength(255)]
        public string WordExample { get; set; }
        [StringLength(255)]
        public string EnglishSenses { get; set; }
        [StringLength(255)]
        public string PinyinOther { get; set; }
        [StringLength(255)]
        public string Pictures { get; set; }
        [StringLength(5)]
        public string LearnOrder { get; set; }
        [StringLength(10)]
        public string FrequencyOrder { get; set; }
        [StringLength(25)]
        public string IdealForms { get; set; }
        [StringLength(25)]
        public string Classification { get; set; }
        public int EtymologyId { get; set; }
        [StringLength(10)]
        public string Unicode { get; set; }
    }
}
