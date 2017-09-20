namespace Etymology.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Etymology
    {
        [Required]
        [StringLength(5)]
        public string Simplified { get; set; }
        [Key]
        [StringLength(5)]
        public string Traditional { get; set; }
        [StringLength(10)]
        public string OldKai { get; set; }
        [StringLength(25)]
        public string SimplificationNewOld { get; set; }
        [StringLength(25)]
        public string Variants796_810 { get; set; }
        [StringLength(255)]
        public string VariantMeaning { get; set; }
        [StringLength(100)]
        public string SimplificationRule { get; set; }
        [StringLength(5)]
        public string SimplificationMultiplicity { get; set; }
        [StringLength(100)]
        public string Meaning { get; set; }
        [StringLength(100)]
        public string CompoundExample { get; set; }
        [StringLength(20)]
        public string RuleBaseObserved { get; set; }
        [StringLength(25)]
        public string RuleBase1753 { get; set; }
        [Column("8105Number")]
        [StringLength(10)]
        public string _8105number { get; set; }
        [Column("8105Char")]
        [StringLength(5)]
        public string _8105char { get; set; }
        [StringLength(10)]
        public string Unicode { get; set; }
        [StringLength(5)]
        public string Radical { get; set; }
        [StringLength(100)]
        public string EnglishMeanings { get; set; }
        [StringLength(50)]
        public string LiuShuStandardName { get; set; }
        [StringLength(50)]
        public string MeaningClass { get; set; }
        [StringLength(50)]
        public string PictographName { get; set; }
        [StringLength(10)]
        public string Pinyin1 { get; set; }
        [StringLength(30)]
        public string PinyinN { get; set; }
        [Column("Jap-Cant")]
        [StringLength(25)]
        public string JapCant { get; set; }
        [StringLength(5)]
        public string FrequencyA { get; set; }
        [StringLength(5)]
        public string FrequencyB { get; set; }
        [StringLength(10)]
        public string VariantSequenceB { get; set; }
        public int? VariantPageA { get; set; }
        [StringLength(10)]
        public string VariantPageB { get; set; }

        [Column(nameof(Etymology))]
        [Required]
        [StringLength(255)]
        public string Decomposition { get; set; }

        internal const string SimplifiedCharacterNumberColumn = "8105xID";

        [Column(SimplifiedCharacterNumberColumn)]
        [StringLength(10)]
        public string SimplifiedCharacterNumber { get; set; }
    }
}
