namespace Etymology.Data.Models
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public partial class EtymologyContext
    {
        public async Task<AnalyzeResult> AnalyzeAsync(char chinese)
        {
            if (char.GetUnicodeCategory(chinese) != UnicodeCategory.OtherLetter)
            {
                throw new ArgumentOutOfRangeException(nameof(chinese), $"{chinese} is not Chinese character.");
            }

            // LINQ to Entities queries creates multiple round trips to database and causes lower performance.
            // Etymology[] etymologies = await this.Etymology
            //   .Where(etymology => etymology.Simplified == @string && etymology.Traditional == @string)
            //   .Concat(this.Etymology.Where(etymology => (etymology.Simplified == @string || etymology.Traditional == @string || etymology.OldKai == @string) && !(etymology.Simplified == @string && etymology.Traditional == @string)))
            //   .ToArrayAsync();
            // Oracle[] oracles = await this.Oracle
            //   .Where(oracle => oracle.Character == @string && oracle.Image != null)
            //   .ToArrayAsync();
            // Bronze[] bronzes = await this.Bronze
            //   .Where(bronze => bronze.Character == @string && bronze.Image != null)
            //   .ToArrayAsync();
            // Liushutong[] liushutongs = await this.Liushutong
            //   .Where(liushutong => liushutong.Character == @string && liushutong.Image != null)
            //   .ToArrayAsync();
            // Seal[] seals = await this.Seal
            //   .Where(seal => seal.Character == @string && seal.Image != null)
            //   .ToArrayAsync();

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
                            DECLARE
	                            @Simplified nvarchar(5),
	                            @Traditional nvarchar(5),
	                            @OldTraditional nvarchar(10),
	                            @Pinyin nvarchar(15),
	                            @Index8105 nvarchar(10),
	                            @SimplificationRule nvarchar(30),
	                            @SimplificationClarified nvarchar(200),
	                            @VariantRule nvarchar(40),
	                            @VariantClarified nvarchar(255),
	                            @AppliedRule nvarchar(30),
	                            @FontRule nvarchar(10),
	                            @Decomposition nvarchar(255),
	                            @DecompositionClarified nvarchar(255),
	                            @OriginalMeaning nvarchar(255),
	                            @Videos nvarchar(50),
	                            @WordExample nvarchar(255),
	                            @EnglishSenses nvarchar(255),
	                            @PinyinOther nvarchar(255),
	                            @Pictures nvarchar(255),
	                            @LearnOrder nvarchar(5),
	                            @FrequencyOrder nvarchar(10),
	                            @IdealForms nvarchar(25),
	                            @Classification nvarchar(25),
	                            @EtymologyId int;

                            SELECT TOP(1)
                                @Simplified = Simplified -- Simplified character
                                ,@Traditional = Traditional -- Traditional character
                                ,@OldTraditional = OldTraditional -- Older traditional characters
                                ,@Pinyin = Pinyin -- Main pronunciation
                                ,@Index8105 = Index8105 -- Simplified character index number
                                ,@SimplificationRule = SimplificationRule -- Simplification rule
                                ,@SimplificationClarified = SimplificationClarified -- Simplification rule explained
                                ,@VariantRule = VariantRule -- Variant rule
                                ,@VariantClarified = VariantClarified -- Variant rule clarification
                                ,@AppliedRule = AppliedRule -- Applied rules
                                ,@FontRule = FontRule -- New font rule
                                ,@Decomposition = Decomposition -- Character decomposition
                                ,@DecompositionClarified = DecompositionClarified -- Decomposition notes
                                ,@OriginalMeaning = OriginalMeaning -- Original meaning
                                ,@EnglishSenses = EnglishSenses -- English senses
                                ,@WordExample = WordExample -- Usage example
                                ,@PinyinOther = PinyinOther -- Other pronunciations
                                ,@Videos = Videos -- Related videos
                                ,@Pictures = Pictures -- Related pictures
                                ,@FrequencyOrder = FrequencyOrder -- Importance by frequency
                                ,@LearnOrder = LearnOrder -- Importance in learning
                                ,@IdealForms = IdealForms -- Ideal ideographs
                                ,@Classification = Classification -- Classification
                            FROM dbo.Etymology 
							WHERE Traditional = @chinese OR Simplified = @chinese OR OldTraditional = @chinese;

							SELECT 
								@Simplified AS Simplified -- Simplified character
                                ,@Traditional AS Traditional -- Traditional character
                                ,@OldTraditional AS OldTraditional -- Older traditional characters
                                ,@Pinyin AS Pinyin -- Main pronunciation
                                ,@Index8105 AS Index8105 -- Simplified character index number
                                ,@SimplificationRule AS SimplificationRule -- Simplification rule
                                ,@SimplificationClarified AS SimplificationClarified -- Simplification rule explained
                                ,@VariantRule AS VariantRule -- Variant rule
                                ,@VariantClarified AS VariantClarified -- Variant rule clarification
                                ,@AppliedRule AS AppliedRule -- Applied rules
                                ,@FontRule AS FontRule -- New font rule
                                ,@Decomposition AS Decomposition -- Character decomposition
                                ,@DecompositionClarified AS DecompositionClarified -- Decomposition notes
                                ,@OriginalMeaning AS OriginalMeaning -- Original meaning
                                ,@EnglishSenses AS EnglishSenses -- English senses
                                ,@WordExample AS WordExample -- Usage example
                                ,@PinyinOther AS PinyinOther -- Other pronunciations
                                ,@Videos AS Videos -- Related videos
                                ,@Pictures AS Pictures -- Related pictures
                                ,@FrequencyOrder AS FrequencyOrder -- Importance by frequency
                                ,@LearnOrder AS LearnOrder -- Importance in learning
                                ,@IdealForms AS IdealForms -- Ideal ideographs
                                ,@Classification AS Classification -- Classification

                            SELECT OracleId, ImageVectorBase64 
							FROM dbo.Oracle 
							WHERE Traditional = @Traditional AND ImageVectorBase64 IS NOT NULL 
							ORDER BY OracleId;

                            SELECT BronzeId, ImageVectorBase64 
							FROM dbo.Bronze 
							WHERE Traditional = @Traditional AND ImageVectorBase64 IS NOT NULL 
							ORDER BY BronzeId;

                            SELECT SealId, ImageVectorBase64 
							FROM dbo.Seal 
							WHERE Traditional = @Traditional AND ImageVectorBase64 IS NOT NULL 
							ORDER BY SealId;

                            SELECT LiushutongId, ImageVectorBase64 
							FROM dbo.Liushutong 
							WHERE Traditional = @Traditional AND ImageVectorBase64 IS NOT NULL 
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
                                    Traditional = reader.ToNullableAndTrim(nameof(Models.Etymology.Traditional)),
                                    Simplified = reader.ToNullableAndTrim(nameof(Models.Etymology.Simplified)),
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
                                });
                            }
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    oracles.Add(new Oracle()
                                    {
                                        OracleId = (int)reader[nameof(Models.Oracle.OracleId)],
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
                                        ImageVectorBase64 = (string)reader[nameof(Models.Seal.ImageVectorBase64)]
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
                                        ImageVectorBase64 = (string)reader[nameof(Models.Liushutong.ImageVectorBase64)]
                                    });
                                }
                            }
                        }
                    }
                }
            });
            return new AnalyzeResult(chinese, etymologies.ToArray(), oracles.ToArray(), bronzes.ToArray(), seals.ToArray(), liushutongs.ToArray());
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
