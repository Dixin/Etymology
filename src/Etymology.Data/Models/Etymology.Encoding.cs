namespace Etymology.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Etymology
    {
        [NotMapped]
        public string SimplifiedInitial { get; set; }

        [NotMapped]
        public int SimplifiedUnicode { get; set; }

        [NotMapped]
        public int TraditionalUnicode { get; set; }

        [NotMapped]
        public string TraditionalUnicodeMessage => this.TraditionalUnicode.ToString("X4");

        [NotMapped]
        public bool HasSimplified => !(this.Simplified.Length > 1 && this.Simplified.StartsWith("p", StringComparison.OrdinalIgnoreCase));

        [NotMapped]
        public string SimplifiedUnicodeMessage => this.SimplifiedUnicode.ToString("X4");
    }
}
