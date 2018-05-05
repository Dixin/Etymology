namespace Etymology.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{" + nameof(Traditional) + "}, {" + nameof(Simplified) + "}, {" + nameof(EtymologyId) + "}, {" + nameof(OldTraditional) + "}")]
    public partial class Etymology : IFormattable
    {
        [NotMapped]
        public string SimplifiedInitial { get; set; }

        [NotMapped]
        public int SimplifiedUnicode { get; set; }

        [NotMapped]
        public string TraditionalInitial { get; set; }

        [NotMapped]
        public int TraditionalUnicode { get; set; }

        [NotMapped]
        public string Prefix => "E";

        [NotMapped]
        public int Id => this.EtymologyId;
    }

    public static class DisplayExtensions
    {
        private static string CharacterPartPrefix = "p";

        public static string ToHex(this int value) =>
            value.ToString("X4");

        public static bool HasSimplified(this Etymology etymology) =>
            !string.IsNullOrEmpty(etymology.Simplified) && !etymology.Simplified.StartsWith(CharacterPartPrefix, StringComparison.OrdinalIgnoreCase);

        public static string Title(this Etymology etymology) =>
            etymology.HasSimplified()
                ? $"{etymology.FormattedId()} {etymology.TraditionalInitial}{etymology.TraditionalUnicode.ToHex()} → {etymology.SimplifiedInitial}{etymology.SimplifiedUnicode.ToHex()}"
                : $"{etymology.FormattedId()} {etymology.TraditionalInitial}{etymology.TraditionalUnicode.ToHex()}";

        public static bool IsNullOrWhiteSpace(this string value) =>
            string.IsNullOrWhiteSpace(value);

        public static string WithNotApplicable(this string value) =>
            string.IsNullOrWhiteSpace(value) || string.Equals(value, "z", StringComparison.OrdinalIgnoreCase)
                ? "Not applicable."
                : value;

        public static string WithNotExsist(this string value) =>
            string.IsNullOrWhiteSpace(value) || string.Equals(value, "z", StringComparison.OrdinalIgnoreCase)
                ? "Not exists."
                : value;

        public static string FormattedId(this IFormattable formattable) => $"{formattable.Prefix.First()}{formattable.Id:00000}";
    }
}
