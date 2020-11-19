#nullable enable
namespace Etymology.Data.Models
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    // Create models:
    // dotnet ef dbcontext scaffold "Server=tcp:{server}.database.windows.net,1433;Initial Catalog={database};Persist Security Info=False;User ID={user};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" Microsoft.EntityFrameworkCore.SqlServer --data-annotations --context EtymologyContext --force --output-dir Models --schema dbo --project Etymology.Data --startup-project Etymology.Data.Console
    public partial class EtymologyContext
    {
        private static IEnumerable<string> FilterStokesForSqlServer(IEnumerable<(string Traditional, int CodePoint)> allTraditional) =>
            allTraditional.Select(traditional => traditional.CodePoint >= 0x31C0 && traditional.CodePoint <= 0x31E3
                ? $"{traditional.Traditional}{traditional.CodePoint:X}"
                : traditional.Traditional);

        public async Task<AnalyzeResult[]> AnalyzeAsync(string chinese, IEnumerable<(string Traditional, int CodePoint)> allTraditional)
        {
            string[] traditionalCharacters = FilterStokesForSqlServer(allTraditional).ToArray();
            if (traditionalCharacters.Length < 1)
            {
                return Array.Empty<AnalyzeResult>();
            }

            // LINQ to Entities queries creates multiple round trips to database and causes lower performance.
            List<Etymology> etymologies = new();
            List<Oracle> oracles = new();
            List<Bronze> bronzes = new();
            List<Liushutong> liushutongs = new();
            List<Seal> seals = new();
            await this.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using DbConnection connection = this.Database.GetDbConnection();
                await connection.OpenAsync();
                await using DbCommand command = connection.CreateCommand();
                DbParameter[] traditionalParameters = traditionalCharacters
                    .Select((traditional, index) =>
                    {
                        DbParameter traditionalParameter = command.CreateParameter();
                        traditionalParameter.ParameterName = $"{nameof(Models.Etymology.Traditional)}{index}";
                        traditionalParameter.Value = traditional;
                        command.Parameters.Add(traditionalParameter);
                        return traditionalParameter;
                    })
                    .ToArray();

                string traditionalParameterText = string.Join(
                    ",",
                    traditionalParameters.Select(traditionalParameter => $"@{traditionalParameter.ParameterName}"));
                command.CommandText = $@"
                    SELECT 
                        Simplified,
                        Traditional,
                        OldTraditional,
                        Pinyin,
                        Index8105,
                        SimplificationRule,
                        SimplificationClarified,
                        VariantRule,
                        VariantClarified,
                        AppliedRule,
                        FontRule,
                        Decomposition,
                        DecompositionClarified,
                        OriginalMeaning,
                        EnglishSenses,
                        WordExample,
                        PinyinOther,
                        Videos,
                        Pictures,
                        FrequencyOrder,
                        LearnOrder,
                        BookIndex,
                        Classification,
	                    EtymologyId
                    FROM dbo.Etymology 
					WHERE Traditional IN ({traditionalParameterText});

                    SELECT OracleId, Oracle.Traditional, ImageVectorBase64 
					FROM dbo.Oracle 
					WHERE ImageVectorBase64 IS NOT NULL AND Traditional IN ({traditionalParameterText})
					ORDER BY OracleId;

                    SELECT BronzeId, Bronze.Traditional, ImageVectorBase64 
					FROM dbo.Bronze 
					WHERE ImageVectorBase64 IS NOT NULL AND Traditional IN ({traditionalParameterText})
					ORDER BY BronzeId;

                    SELECT SealId, Seal.Traditional, ImageVectorBase64, ShuowenTraditional
					FROM dbo.Seal 
					WHERE ImageVectorBase64 IS NOT NULL AND Traditional IN ({traditionalParameterText})
					ORDER BY SealId;

                    SELECT LiushutongId, Liushutong.Traditional, ImageVectorBase64 
					FROM dbo.Liushutong 
					WHERE ImageVectorBase64 IS NOT NULL AND Traditional IN ({traditionalParameterText})
					ORDER BY LiushutongId;";

                await using DbDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    etymologies.Add(new Etymology()
                    {
                        Simplified = reader.ToStringAndTrim(nameof(Models.Etymology.Simplified)),
                        Traditional = reader.ToStringAndTrim(nameof(Models.Etymology.Traditional)),
                        OldTraditional = reader.ToStringAndTrim(nameof(Models.Etymology.OldTraditional)),
                        Pinyin = reader.ToStringAndTrim(nameof(Models.Etymology.Pinyin)),
                        Index8105 = reader.ToStringAndTrim(nameof(Models.Etymology.Index8105)),
                        SimplificationRule = reader.ToStringAndTrim(nameof(Models.Etymology.SimplificationRule)),
                        SimplificationClarified = reader.ToStringAndTrim(nameof(Models.Etymology.SimplificationClarified)),
                        VariantRule = reader.ToStringAndTrim(nameof(Models.Etymology.VariantRule)),
                        VariantClarified = reader.ToStringAndTrim(nameof(Models.Etymology.VariantClarified)),
                        AppliedRule = reader.ToStringAndTrim(nameof(Models.Etymology.AppliedRule)),
                        FontRule = reader.ToStringAndTrim(nameof(Models.Etymology.FontRule)),
                        Decomposition = reader.ToStringAndTrim(nameof(Models.Etymology.Decomposition)),
                        DecompositionClarified = reader.ToStringAndTrim(nameof(Models.Etymology.DecompositionClarified)),
                        OriginalMeaning = reader.ToStringAndTrim(nameof(Models.Etymology.OriginalMeaning)),
                        EnglishSenses = reader.ToStringAndTrim(nameof(Models.Etymology.EnglishSenses)),
                        WordExample = reader.ToStringAndTrim(nameof(Models.Etymology.WordExample)),
                        PinyinOther = reader.ToStringAndTrim(nameof(Models.Etymology.PinyinOther)),
                        Videos = reader.ToStringAndTrim(nameof(Models.Etymology.Videos)),
                        Pictures = reader.ToStringAndTrim(nameof(Models.Etymology.Pictures)),
                        FrequencyOrder = reader.ToStringAndTrim(nameof(Models.Etymology.FrequencyOrder)),
                        LearnOrder = reader.ToStringAndTrim(nameof(Models.Etymology.LearnOrder)),
                        BookIndex = reader.ToStringAndTrim(nameof(Models.Etymology.BookIndex)),
                        Classification = reader.ToStringAndTrim(nameof(Models.Etymology.Classification)),
                        EtymologyId = (int)reader[nameof(Models.Etymology.EtymologyId)],
                    });
                }
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        oracles.Add(new Oracle()
                        {
                            OracleId = (int)reader[nameof(Models.Oracle.OracleId)],
                            Traditional = (string)reader[nameof(Models.Oracle.Traditional)],
                            ImageVectorBase64 = (string)reader[nameof(Models.Oracle.ImageVectorBase64)]
                        });
                    }
                }
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        bronzes.Add(new Bronze()
                        {
                            BronzeId = (int)reader[nameof(Models.Bronze.BronzeId)],
                            Traditional = (string)reader[nameof(Models.Bronze.Traditional)],
                            ImageVectorBase64 = (string)reader[nameof(Models.Bronze.ImageVectorBase64)]
                        });
                    }
                }
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        seals.Add(new Seal()
                        {
                            SealId = (int)reader[nameof(Models.Seal.SealId)],
                            Traditional = (string)reader[nameof(Models.Seal.Traditional)],
                            ImageVectorBase64 = (string)reader[nameof(Models.Seal.ImageVectorBase64)],
                            ShuowenTraditional = reader.ToStringAndTrim(nameof(Models.Seal.ShuowenTraditional))
                        });
                    }
                }
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        liushutongs.Add(new Liushutong()
                        {
                            LiushutongId = (int)reader[nameof(Models.Liushutong.LiushutongId)],
                            Traditional = (string)reader[nameof(Models.Liushutong.Traditional)],
                            ImageVectorBase64 = (string)reader[nameof(Models.Liushutong.ImageVectorBase64)]
                        });
                    }
                }
            });
            return etymologies
                .Select(etymology => new AnalyzeResult(
                    chinese,
                    etymology,
                    oracles.Where(character => string.Equals(character.Traditional, etymology.Traditional, StringComparison.Ordinal)).ToArray(),
                    bronzes.Where(character => string.Equals(character.Traditional, etymology.Traditional, StringComparison.Ordinal)).ToArray(),
                    seals.Where(character => string.Equals(character.Traditional, etymology.Traditional, StringComparison.Ordinal)).ToArray(),
                    liushutongs.Where(character => string.Equals(character.Traditional, etymology.Traditional, StringComparison.Ordinal)).ToArray()))
                .OrderByDescending(result => result.CharacterCount)
                .ToArray();
        }
    }

    internal static class DbDataReaderExtensions
    {
        [return: MaybeNull]
        internal static T ToNullable<T>(this DbDataReader reader, string column) where T : class
        {
            object value = reader[column];
            return value is DBNull || value is null ? null : (T)value;
        }

        internal static string ToStringAndTrim(this DbDataReader reader, string column)
        {
            object value = reader[column];
            return value is DBNull || value is null ? string.Empty : ((string)value).Trim();
        }
    }
}
