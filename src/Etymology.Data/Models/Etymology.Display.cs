namespace Etymology.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;

    [DebuggerDisplay("{" + nameof(Traditional) + "}, {" + nameof(Simplified) + "}, {" + nameof(EtymologyId) + "}, {" + nameof(OldTraditional) + "}")]
    public partial class Etymology
    {
        [NotMapped]
        public string SimplifiedInitial { get; set; }

        [NotMapped]
        public int SimplifiedUnicode { get; set; }

        [NotMapped]
        public int TraditionalUnicode { get; set; }
    }

    public static class EtymologyExtensions
    {
        public static string ToHex(this int value) =>
            value.ToString("X4");

        public static bool HasSimplified(this Etymology etymology) =>
            string.Equals(etymology.Simplified, etymology.SimplifiedInitial, StringComparison.Ordinal)
            || etymology.Simplified.Length - etymology.SimplifiedInitial.Length == 1
            && int.TryParse(etymology.Simplified.Substring(etymology.SimplifiedInitial.Length), out int value)
            && value < 10;

        public static string Title(this Etymology etymology) =>
            etymology.HasSimplified()
                ? $"{etymology.Traditional}(U+{etymology.TraditionalUnicode.ToHex()}) → {etymology.Simplified}(U+{etymology.SimplifiedUnicode.ToHex()})"
                : $"{etymology.Traditional}(U+{etymology.TraditionalUnicode.ToHex()})";

        public static string Message(this string value) =>
            string.Equals(value, "z", StringComparison.OrdinalIgnoreCase)
                ? "Not exist."
                : value;
    }
}
