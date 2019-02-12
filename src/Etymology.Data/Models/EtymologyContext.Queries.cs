namespace Etymology.Data.Models
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

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
            List<Etymology> etymologies = new List<Etymology>();
            List<Oracle> oracles = new List<Oracle>();
            List<Bronze> bronzes = new List<Bronze>();
            List<Liushutong> liushutongs = new List<Liushutong>();
            List<Seal> seals = new List<Seal>();
            await this.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using (DbConnection connection = this.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (DbCommand command = connection.CreateCommand())
                    {
                        DbParameter[] traditionalParameters = traditionalCharacters
                            .Select((traditional, index) =>
                            {
                                DbParameter traditionalParameter = command.CreateParameter();
                                traditionalParameter.ParameterName =
                                    $"{nameof(Models.Etymology.Traditional)}{index}";
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
                                IdealForms,
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

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                etymologies.Add(new Etymology()
                                {
                                    Simplified = reader.ToNullableAndTrim(nameof(Models.Etymology.Simplified)),
                                    Traditional = reader.ToNullableAndTrim(nameof(Models.Etymology.Traditional)),
                                    OldTraditional = reader.ToNullableAndTrim(nameof(Models.Etymology.OldTraditional)),
                                    Pinyin = reader.ToNullableAndTrim(nameof(Models.Etymology.Pinyin)),
                                    Index8105 = reader.ToNullableAndTrim(nameof(Models.Etymology.Index8105)),
                                    SimplificationRule = reader.ToNullableAndTrim(nameof(Models.Etymology.SimplificationRule)),
                                    SimplificationClarified = reader.ToNullableAndTrim(nameof(Models.Etymology.SimplificationClarified)),
                                    VariantRule = reader.ToNullableAndTrim(nameof(Models.Etymology.VariantRule)),
                                    VariantClarified = reader.ToNullableAndTrim(nameof(Models.Etymology.VariantClarified)),
                                    AppliedRule = reader.ToNullableAndTrim(nameof(Models.Etymology.AppliedRule)),
                                    FontRule = reader.ToNullableAndTrim(nameof(Models.Etymology.FontRule)),
                                    Decomposition = reader.ToNullableAndTrim(nameof(Models.Etymology.Decomposition)),
                                    DecompositionClarified = reader.ToNullableAndTrim(nameof(Models.Etymology.DecompositionClarified)),
                                    OriginalMeaning = reader.ToNullableAndTrim(nameof(Models.Etymology.OriginalMeaning)),
                                    EnglishSenses = reader.ToNullableAndTrim(nameof(Models.Etymology.EnglishSenses)),
                                    WordExample = reader.ToNullableAndTrim(nameof(Models.Etymology.WordExample)),
                                    PinyinOther = reader.ToNullableAndTrim(nameof(Models.Etymology.PinyinOther)),
                                    Videos = reader.ToNullableAndTrim(nameof(Models.Etymology.Videos)),
                                    Pictures = reader.ToNullableAndTrim(nameof(Models.Etymology.Pictures)),
                                    FrequencyOrder = reader.ToNullableAndTrim(nameof(Models.Etymology.FrequencyOrder)),
                                    LearnOrder = reader.ToNullableAndTrim(nameof(Models.Etymology.LearnOrder)),
                                    IdealForms = reader.ToNullableAndTrim(nameof(Models.Etymology.IdealForms)),
                                    Classification = reader.ToNullableAndTrim(nameof(Models.Etymology.Classification)),
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
                                        ShuowenTraditional = reader.ToNullableAndTrim(nameof(Models.Seal.ShuowenTraditional))
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
                        }
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
        internal static T ToNullable<T>(this DbDataReader reader, string column) where T : class
        {
            object value = reader[column];
            return value is DBNull || value is null ? null : (T)value;
        }

        internal static string ToNullableAndTrim(this DbDataReader reader, string column)
        {
            object value = reader[column];
            return value is DBNull || value is null ? null : ((string)value).Trim();
        }
    }
}
