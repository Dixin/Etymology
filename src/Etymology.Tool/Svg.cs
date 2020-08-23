namespace Etymology.Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Etymology.Data.Models;
    using Microsoft.EntityFrameworkCore;

    internal static class Svg
    {
        internal static int Save(SvgOptions svgOptions)
        {
            string[] oracleCharacters = Directory.GetFiles(svgOptions.Directory, "j*.svg", SearchOption.AllDirectories);
            Array.Sort(oracleCharacters);
            string[] bronzeCharacters = Directory.GetFiles(svgOptions.Directory, "b*.svg", SearchOption.AllDirectories);
            Array.Sort(bronzeCharacters);
            string[] sealCharacters = Directory.GetFiles(svgOptions.Directory, "s*.svg", SearchOption.AllDirectories);
            Array.Sort(sealCharacters);
            string[] liushutongCharacters = Directory.GetFiles(svgOptions.Directory, "l*.svg", SearchOption.AllDirectories);
            Array.Sort(liushutongCharacters);

            using EtymologyContext database = Database(svgOptions.Connection);
            if (!(Update<Oracle>(database, oracleCharacters)
                && Update<Bronze>(database, bronzeCharacters)
                && Update<Seal>(database, sealCharacters)
                && Update<Liushutong>(database, liushutongCharacters)))
            {
                return 1;
            }

            int count = database.SaveChanges();
            Console.WriteLine($"{count} files saved to database.");
            return 0;
        }

        private static EtymologyContext Database(string connection)
        {
            return new EtymologyContext(new DbContextOptionsBuilder<EtymologyContext>().UseSqlServer(
                connection,
                options => options
                    .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)).Options);
        }

        private static bool Update<T>(EtymologyContext database, string[] images)
            where T : class, ICharacter
        {
            if (images.Any())
            {
                T[] entities = database.Set<T>().ToArray();
                Dictionary<int, T> dictionary = entities.ToDictionary(entity => entity.Id);
                foreach (string image in images)
                {
                    if (int.TryParse(Path.GetFileNameWithoutExtension(image).Substring(1), out int id))
                    {
                        if (!dictionary.ContainsKey(id))
                        {
                            Console.WriteLine($"Character not in database: {image}.");
                            return false;
                        }

                        dictionary[id].ImageVector = File.ReadAllText(image);
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect file is ignored: {image}.");

                        // return false;
                    }
                }
            }

            return true;
        }
    }
}
