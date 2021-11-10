#nullable enable
namespace Etymology.Data.Models;

public record AnalyzeResult
{
    public AnalyzeResult(string chinese, Etymology etymologies, IList<Oracle> oracles, IList<Bronze> bronzes, IList<Seal> seals, IList<Liushutong> liushutongs)
    {
        this.Chinese = chinese;
        this.Etymology = etymologies ?? throw new ArgumentNullException(nameof(etymologies));
        this.Oracles = oracles ?? throw new ArgumentNullException(nameof(oracles));
        this.Bronzes = bronzes ?? throw new ArgumentNullException(nameof(bronzes));
        this.Seals = seals ?? throw new ArgumentNullException(nameof(seals));
        this.Liushutongs = liushutongs ?? throw new ArgumentNullException(nameof(liushutongs));
        this.CharacterCount = oracles.Count + bronzes.Count + seals.Count + liushutongs.Count;
    }

    public string Chinese { get; }

    public Etymology Etymology { get; }

    public IList<Oracle> Oracles { get; }

    public IList<Bronze> Bronzes { get; }

    public IList<Seal> Seals { get; }

    public IList<Liushutong> Liushutongs { get; }

    public int CharacterCount { get; }

    public void Deconstruct(out string chinese, out Etymology etymologies, out IList<Oracle> oracles, out IList<Bronze> bronzes, out IList<Seal> seals, out IList<Liushutong> liushutongs)
    {
        chinese = this.Chinese;
        etymologies = this.Etymology;
        oracles = this.Oracles;
        bronzes = this.Bronzes;
        seals = this.Seals;
        liushutongs = this.Liushutongs;
    }
}