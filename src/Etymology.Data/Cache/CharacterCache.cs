namespace Etymology.Data.Cache
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Etymology.Common;
    using Etymology.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public interface ICharacterCache
    {
        Task Initialize(EtymologyContext etymologyContext);

        IEnumerable<(string Traditional, int CodePoint)> AllTraditional(int codePoint);
    }

    public class CharacterCache : ICharacterCache
    {
        private Dictionary<int, object> traditional;

        private ILookup<int, int> simplifiedToTraditional;

        private ILookup<int, int> oldTraditionalToTraditional;

        public async Task Initialize(EtymologyContext etymologyContext)
        {
            (int Traditional, int Simplified, int[] OldTraditional)[] etymologies = (await etymologyContext.Etymology.Select(etymology => new { etymology.Traditional, etymology.Simplified, etymology.OldTraditional }).ToArrayAsync())
                .Where(etymology => !string.IsNullOrWhiteSpace(etymology.Simplified))
                .Select(etymology =>
                (
                    Traditional: char.ConvertToUtf32(etymology.Traditional, 0),
                    Simplified: char.ConvertToUtf32(etymology.Simplified.Characters().First(), 0),
                    OldTraditional: string.IsNullOrWhiteSpace(etymology.OldTraditional)
                        ? null
                        : etymology.OldTraditional.Characters().Select(old => char.ConvertToUtf32(old, 0)).ToArray())
                )
                .ToArray();
            this.traditional = etymologies.ToDictionary(etymology => etymology.Traditional, etymology => (object)null);
            this.simplifiedToTraditional = etymologies.ToLookup(etymology => etymology.Simplified, etymology => etymology.Traditional);
            this.oldTraditionalToTraditional = etymologies
                .Where(etymology => etymology.OldTraditional != null)
                .SelectMany(etymology => etymology.OldTraditional, (etymology, old) => (Old: old, Traditional: etymology.Traditional))
                .ToLookup(etymology => etymology.Old, etymology => etymology.Traditional);
        }

        public IEnumerable<(string Traditional, int CodePoint)> AllTraditional(int codePoint)
        {
            List<int> allTraditional = this.simplifiedToTraditional[codePoint].ToList();
            allTraditional.AddRange(this.oldTraditionalToTraditional[codePoint]);
            if (this.traditional.ContainsKey(codePoint))
            {
                allTraditional.Add(codePoint);
            }

            return allTraditional.Distinct().Select(traditional => (char.ConvertFromUtf32(traditional), codePage: traditional));
        }
    }
}
