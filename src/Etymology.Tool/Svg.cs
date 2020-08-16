namespace Etymology.Tool
{
    using CommandLine;
    using Etymology.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [Verb("svgtodb", HelpText = "Add SVG files to database.")]
    internal class SvgOptions
    {
        [Option('d', "directory", Required = false, HelpText = "Save all SVG files in the directory to database.")]
        public string Directory { get; set; } = @"d:\test";

        [Option('c', "connection", Required = false, HelpText = "Connection string to database.")]
        public string Connection { get; set; } = "Server=localhost;Initial Catalog=chineseetymology;User ID=sa;Password=ftSq1@zure;MultipleActiveResultSets=False;Connection Timeout=30;";
    }

    internal class Svg
    {
        private static EtymologyContext Database(string connection)
        {
            return new EtymologyContext(new DbContextOptionsBuilder<EtymologyContext>().UseSqlServer(
                connection,
                options => options
                    .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)).Options);
        }
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

        private static bool Update<T>(EtymologyContext database, string[] images) where T : class, ICharacter
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
