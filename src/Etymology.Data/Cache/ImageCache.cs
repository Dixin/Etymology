namespace Etymology.Data.Cache
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Etymology.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    public class ImageCache
    {
        private readonly string cacheRoot;

        private readonly EtymologyContext context;

        public ImageCache(EtymologyContext context, string cacheRoot)
        {
            this.context = context;
            this.cacheRoot = cacheRoot;
        }

        public async Task SaveWithRetryAsync() => 
            await Retry.FixedIntervalAsync(
                async () =>
                {
                    await this.context.BronzeImages().ForEachAsync(character =>
                        SaveCharacterWithRetry(this.cacheRoot, character));
                    await this.context.LiushutongImages().ForEachAsync(bronze =>
                        SaveCharacterWithRetry(this.cacheRoot, bronze));
                    await this.context.OracleImages().ForEachAsync(character =>
                        SaveCharacterWithRetry(this.cacheRoot, character));
                    await this.context.SealImages().ForEachAsync(character =>
                        SaveCharacterWithRetry(this.cacheRoot, character));
                },
                retryCount: 10,
                retryInterval: TimeSpan.FromSeconds(10));

        private static void SaveCharacterWithRetry(string cacheRoot, ICharacter character) => 
            Retry.FixedInterval(
                () =>
                {
                    string path = Path.Combine(cacheRoot, character.Path());
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllBytes(path, character.Image);
                },
                retryInterval: TimeSpan.Zero,
                retryCount: 3);
    }
}
