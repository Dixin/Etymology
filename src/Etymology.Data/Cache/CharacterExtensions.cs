namespace Etymology.Data.Cache
{
    using Etymology.Data.Models;

    internal static class CharacterExtensions
    {
        internal static string Path(this ICharacter character) =>
            System.IO.Path.Combine(character.Prefix, $"{character.Id / 10000}0000", $"{character.Id / 100:000}00", $"{character.FormattedId()}.gif");
    }
}
