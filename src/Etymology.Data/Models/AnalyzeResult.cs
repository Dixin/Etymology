#nullable enable
namespace Etymology.Data.Models;

public record AnalyzeResult(string Chinese, Etymology Etymologies, IList<Oracle> Oracles, IList<Bronze> Bronzes, IList<Seal> Seals, IList<Liushutong> Liushutongs)
{
    public int CharacterCount => this.Oracles.Count + this.Bronzes.Count + this.Seals.Count + this.Liushutongs.Count;
}