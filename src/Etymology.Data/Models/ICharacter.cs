namespace Etymology.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IFormattable
    {
        [NotMapped]
        string Prefix { get; }

        [NotMapped]
        int Id { get; }
    }

    public interface ICharacter : IFormattable
    {
        string Traditional { get; set; }

        string ImageBase64 { get; set; }

        string ImageVector { get; set; }
    }

    public partial class Bronze : ICharacter
    {
        [NotMapped]
        public string Prefix => "B";

        [NotMapped]
        public int Id => this.BronzeId;
    }

    public partial class Liushutong : ICharacter
    {
        [NotMapped]
        public string Prefix => "L";

        [NotMapped]
        public int Id => this.LiushutongId;
    }

    public partial class Oracle : ICharacter
    {
        [NotMapped]
        public string Prefix => "J";

        [NotMapped]
        public int Id => this.OracleId;
    }

    public partial class Seal : ICharacter
    {
        [NotMapped]
        public string Prefix => "S";

        [NotMapped]
        public int Id => this.SealId;
    }
}
