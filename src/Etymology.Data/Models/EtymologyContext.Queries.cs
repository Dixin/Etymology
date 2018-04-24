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
        public async Task<AnalyzeResult[]> AnalyzeAsync(string chinese)
        {
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
                        command.CommandText = @"
                            DECLARE @Etymology TABLE
							(
	                            Simplified nvarchar(5) COLLATE Chinese_Simplified_Pinyin_100_CS_AS_KS_WS_SC,
                                SimplifiedInitial nvarchar(2) COLLATE Chinese_Simplified_Pinyin_100_CS_AS_KS_WS_SC,
                                SimplifiedUnicode int,
	                            Traditional nvarchar(5) COLLATE Chinese_Traditional_Pinyin_100_CS_AS_KS_WS_SC,
                                TraditionalUnicode int,
	                            OldTraditional nvarchar(10) COLLATE Chinese_Traditional_Pinyin_100_CS_AS_KS_WS_SC,
	                            Pinyin nvarchar(15),
	                            Index8105 nvarchar(10),
	                            SimplificationRule nvarchar(30),
	                            SimplificationClarified nvarchar(200),
	                            VariantRule nvarchar(40),
	                            VariantClarified nvarchar(255),
	                            AppliedRule nvarchar(30),
	                            FontRule nvarchar(10),
	                            Decomposition nvarchar(255),
	                            DecompositionClarified nvarchar(255),
	                            OriginalMeaning nvarchar(255),
	                            Videos nvarchar(50),
	                            WordExample nvarchar(255),
	                            EnglishSenses nvarchar(255),
	                            PinyinOther nvarchar(255),
	                            Pictures nvarchar(255),
	                            LearnOrder nvarchar(5),
	                            FrequencyOrder nvarchar(10),
	                            IdealForms nvarchar(25),
	                            Classification nvarchar(25),
	                            EtymologyId int
							);

							INSERT INTO @Etymology
							(
								Simplified,
                                SimplifiedInitial,
                                SimplifiedUnicode,
                                Traditional,
                                TraditionalUnicode,
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
							)
                            SELECT 
                                Simplified,
                                SUBSTRING(Simplified, 1, 1) AS SimplifiedInitial,
                                UNICODE(SUBSTRING(Simplified, 1, 1)) AS SimplifiedUnicode,
                                Traditional,
                                UNICODE(Traditional) AS TraditionalUnicode,
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
							WHERE Traditional = @chinese OR Simplified LIKE @chinese + N'%' OR OldTraditional LIKE N'%' + @chinese + N'%';

							SELECT 
								Simplified, -- Simplified character
                                SimplifiedInitial,
                                SimplifiedUnicode,
                                Traditional, -- Traditional character
                                TraditionalUnicode,
                                OldTraditional, -- Older traditional characters
                                Pinyin, -- Main pronunciation
                                Index8105, -- Simplified character index number
                                SimplificationRule, -- Simplification rule
                                SimplificationClarified, -- Simplification rule explained
                                VariantRule, -- Variant rule
                                VariantClarified, -- Variant rule clarification
                                AppliedRule, -- Applied rules
                                FontRule, -- New font rule
                                Decomposition, -- Character decomposition
                                DecompositionClarified, -- Decomposition notes
                                OriginalMeaning, -- Original meaning
                                EnglishSenses, -- English senses
                                WordExample, -- Usage example
                                PinyinOther, -- Other pronunciations
                                Videos, -- Related videos
                                Pictures, -- Related pictures
                                FrequencyOrder, -- Importance by frequency
                                LearnOrder, -- Importance in learning
                                IdealForms, -- Ideal ideographs
                                Classification, -- Classification
	                            EtymologyId
							FROM @Etymology;

                            SELECT OracleId, Oracle.Traditional, ImageVectorBase64 
							FROM dbo.Oracle 
							INNER JOIN @Etymology AS Etymology ON Oracle.Traditional = Etymology.Traditional
							WHERE ImageVectorBase64 IS NOT NULL 
							ORDER BY OracleId;

                            SELECT BronzeId, Bronze.Traditional, ImageVectorBase64 
							FROM dbo.Bronze 
							INNER JOIN @Etymology AS Etymology ON Bronze.Traditional = Etymology.Traditional
							WHERE ImageVectorBase64 IS NOT NULL 
							ORDER BY BronzeId;

                            SELECT SealId, Seal.Traditional, ImageVectorBase64, ShuowenTraditional
							FROM dbo.Seal 
							INNER JOIN @Etymology AS Etymology ON Seal.Traditional = Etymology.Traditional
							WHERE ImageVectorBase64 IS NOT NULL 
							ORDER BY SealId;

                            SELECT LiushutongId, Liushutong.Traditional, ImageVectorBase64 
							FROM dbo.Liushutong 
							INNER JOIN @Etymology AS Etymology ON Liushutong.Traditional = Etymology.Traditional
							WHERE ImageVectorBase64 IS NOT NULL 
							ORDER BY LiushutongId;";
                        DbParameter characterParameter = command.CreateParameter();
                        characterParameter.ParameterName = nameof(chinese);
                        characterParameter.Value = chinese;
                        command.Parameters.Add(characterParameter);
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                etymologies.Add(new Etymology()
                                {
                                    Simplified = reader.ToNullableAndTrim(nameof(Models.Etymology.Simplified)),
                                    SimplifiedInitial = reader.ToNullableAndTrim(nameof(Models.Etymology.SimplifiedInitial)),
                                    SimplifiedUnicode = (int)reader[nameof(Models.Etymology.SimplifiedUnicode)],
                                    Traditional = reader.ToNullableAndTrim(nameof(Models.Etymology.Traditional)),
                                    TraditionalUnicode = (int)reader[nameof(Models.Etymology.TraditionalUnicode)],
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
                .ToArray();
        }

        public IQueryable<Bronze> BronzeImages() =>
            this.Bronze.Where(bonze => bonze.ImageBase64 != null);

        public IQueryable<Liushutong> LiushutongImages() =>
            this.Liushutong.Where(liushutong => liushutong.ImageBase64 != null);

        public IQueryable<Oracle> OracleImages() =>
            this.Oracle.Where(oracle => oracle.ImageBase64 != null);

        public IQueryable<Seal> SealImages() =>
            this.Seal.Where(seal => seal.ImageBase64 != null);
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
