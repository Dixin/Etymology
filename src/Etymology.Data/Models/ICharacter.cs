namespace Etymology.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Xml.Linq;
    using global::Etymology.Data.Common;

    public interface ICharacter
    {
        [NotMapped]
        string Prefix { get; }

        [NotMapped]
        int Id { get; }

        string ImageBase64 { get; set; }

        string ImageVector { get; set; }

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

    public static class CharacterExtensions
    {
        public static string ImageVectorBase64(this ICharacter character)
        {
            try
            {
                XDocument svg = XDocument.Parse(character.ImageVector);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(svg.Root.ToString(SaveOptions.DisableFormatting)));
            }
            catch (Exception exception) when (exception.IsNotCritical())
            {
                return string.Empty;
            }
        }
    }
}
