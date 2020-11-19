#nullable enable
namespace Etymology.Data.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Etymology.Common;
    using Etymology.Data.Models;

    public interface ICharacterCache
    {
        IEnumerable<(string Traditional, int CodePoint)> AllTraditional(int codePoint);
    }

    public class CharacterCache : ICharacterCache
    {
        private static Dictionary<int, object?> traditional = new();

        private static ILookup<int, int> simplifiedToTraditional = Array.Empty<int>().ToLookup(value => value, value => value);

        private static ILookup<int, int> oldTraditionalToTraditional = Array.Empty<int>().ToLookup(value => value, value => value);

        public CharacterCache(EtymologyContext etymologyContext)
        {
            if (traditional.Any())
            {
                return;
            }

            (int Traditional, int Simplified, int[] OldTraditional)[] etymologies = etymologyContext
                .Etymology
                .Select(etymology => new { etymology.Traditional, etymology.Simplified, etymology.OldTraditional })
                .AsEnumerable()
                .Where(etymology => !string.IsNullOrWhiteSpace(etymology.Simplified))
                .Select(etymology =>
                (
                    Traditional: char.ConvertToUtf32(etymology.Traditional, 0),
                    Simplified: char.ConvertToUtf32(etymology.Simplified.Characters().First(), 0),
                    OldTraditional: string.IsNullOrWhiteSpace(etymology.OldTraditional)
                        ? Array.Empty<int>()
                        : etymology.OldTraditional.Characters().Select(old => char.ConvertToUtf32(old, 0)).ToArray()
                ))
                .ToArray();
            traditional = etymologies.ToDictionary(etymology => etymology.Traditional, _ => (object?)null);
            simplifiedToTraditional = etymologies.ToLookup(etymology => etymology.Simplified, etymology => etymology.Traditional);
            oldTraditionalToTraditional = etymologies
                .Where(etymology => etymology.OldTraditional is not null)
                .SelectMany(etymology => etymology.OldTraditional, (etymology, old) => (Old: old, Traditional: etymology.Traditional))
                .ToLookup(etymology => etymology.Old, etymology => etymology.Traditional);
        }

        public IEnumerable<(string Traditional, int CodePoint)> AllTraditional(int codePoint)
        {
            List<int> allTraditional = simplifiedToTraditional[codePoint].ToList();
            allTraditional.AddRange(oldTraditionalToTraditional[codePoint]);
            if (traditional.ContainsKey(codePoint))
            {
                allTraditional.Add(codePoint);
            }

            return allTraditional
                .Distinct()
                .Select(traditional => (Traditional: char.ConvertFromUtf32(traditional), CodePoint: traditional));
        }
    }
}
