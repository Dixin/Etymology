namespace Etymology.Data.Models
{
    using System;

    public class AnalyzeResult
    {
        public AnalyzeResult(char chinese, Etymology[] etymologies, Oracle[] oracles, Bronze[] bronzes, Seal[] seals, Liushutong[] liushutongs)
        {
            this.Chinese = chinese;
            this.Etymologies = etymologies ?? throw new ArgumentNullException(nameof(etymologies));
            this.Oracles = oracles ?? throw new ArgumentNullException(nameof(oracles));
            this.Bronzes = bronzes ?? throw new ArgumentNullException(nameof(bronzes));
            this.Seals = seals ?? throw new ArgumentNullException(nameof(seals));
            this.Liushutongs = liushutongs ?? throw new ArgumentNullException(nameof(liushutongs));
        }

        public char Chinese { get; }

        public Etymology[] Etymologies { get; }

        public Oracle[] Oracles { get; }

        public Bronze[] Bronzes { get; }

        public Seal[] Seals { get; }

        public Liushutong[] Liushutongs { get; }
    }
}