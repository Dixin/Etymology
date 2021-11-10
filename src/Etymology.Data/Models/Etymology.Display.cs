namespace Etymology.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;
using global::Etymology.Common;

[DebuggerDisplay("{" + nameof(Traditional) + "}, {" + nameof(Simplified) + "}, {" + nameof(EtymologyId) + "}, {" + nameof(OldTraditional) + "}")]
public partial class Etymology : IFormattable
{
    [NotMapped]
    public string SimplifiedInitial => this.Simplified.Characters().First();

    [NotMapped]
    public int SimplifiedUnicode => char.ConvertToUtf32(this.Simplified, 0);

    [NotMapped]
    public string TraditionalInitial => this.Traditional.Characters().First();

    [NotMapped]
    public int TraditionalUnicode => char.ConvertToUtf32(this.Traditional, 0);

    [NotMapped]
    public string Prefix => "E";

    [NotMapped]
    public int Id => this.EtymologyId;
}

public static class DisplayExtensions
{
    public static string ToHex(this int value) =>
        value.ToString("X4");

    public static bool HasSimplified(this Etymology etymology) =>
        !string.IsNullOrEmpty(etymology.Simplified) && etymology.Simplified.Characters().First().IsSingleChineseCharacter();

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

    public static string WithNotExist(this string value) =>
        string.IsNullOrWhiteSpace(value) || string.Equals(value, "z", StringComparison.OrdinalIgnoreCase)
            ? "Not exists."
            : value;

    public static string FormattedId(this IFormattable formattable) => $"{formattable.Prefix.First()}{formattable.Id:00000}";
}