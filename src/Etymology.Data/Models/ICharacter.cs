namespace Etymology.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface ICharacter
    {
        [NotMapped]
        string Prefix { get; }

        [NotMapped]
        int Id { get; }

        [NotMapped]
        byte[] Image { get; }

        [NotMapped]
        string FormattedId { get; }
    }

    public partial class Bronze : ICharacter
    {
        [NotMapped]
        public string Prefix => "B";

        [NotMapped]
        public int Id => this.BronzeId;

        [NotMapped]
        public string FormattedId => $"{this.Prefix}{this.Id:00000}";
    }

    public partial class Liushutong : ICharacter
    {
        [NotMapped]
        public string Prefix => "L";

        [NotMapped]
        public int Id => this.LiushutongId;

        [NotMapped]
        public string FormattedId => $"{this.Prefix}{this.Id:00000}";
    }

    public partial class Oracle : ICharacter
    {
        [NotMapped]
        public string Prefix => "J";

        [NotMapped]
        public int Id => this.OracleId;

        [NotMapped]
        public string FormattedId => $"{this.Prefix}{this.Id:00000}";
    }

    public partial class Seal : ICharacter
    {
        [NotMapped]
        public string Prefix => "S";

        [NotMapped]
        public int Id => this.SealId;

        [NotMapped]
        public string FormattedId => $"{this.Prefix}{this.Id:00000}";
    }
}
