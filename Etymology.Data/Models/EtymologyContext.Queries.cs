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
                throw new ArgumentOutOfRangeException(nameof(chinese));
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
                            select 
                                [Traditional]
                                ,[Simplified]
                                ,[OldKai] -- older character 
                                ,[Variants796_810] -- variant rule
                                ,[VariantMeaning] -- meanings of variants
                                ,[SimplificationRule] -- simplification rule
                                ,[SimplificationNewOld] -- modern invention 
                                ,[RuleBase1753] -- compound rule
                                ,[RuleBaseObserved] -- applied rules
                                ,[Etymology] -- decomposition
                                ,[Meaning] -- original meaning
                                ,[CompoundExample] -- example in use
                                ,[8105xID] -- simplified character number
                                ,[EnglishMeanings] -- modern meanings
                                ,[LiuShuStandardName] -- standard name of character
                                ,[MeaningClass] -- class of character
                                ,[PictographName] -- original 
                                ,[Pinyin1] -- main pronunciation
                                ,[PinyinN] -- other pronunciations 
                            from
                            ((select * from dbo.Etymology where Simplified = @chinese and traditional = @chinese)
                            Union all
                            (select * from dbo.Etymology where (Simplified = @chinese or traditional = @chinese or OldKai = @chinese) and not (Simplified = @chinese and traditional = @chinese)))
                            as etymology;

                            declare @traditional nvarchar(5);
                            select top(1) @traditional = [Traditional] from dbo.Etymology where Simplified = @chinese or traditional = @chinese or OldKai = @chinese;

                            select OracleId, Image from dbo.Oracle where Character = @traditional and image is not null order by OracleId;

                            select BronzeId, Image from dbo.Bronze where Character = @traditional and image is not null order by BronzeId;

                            select SealId, Image, ShuowenSimplified from dbo.Seal where Character = @traditional and image is not null order by SealId;

                            select LiushutongId, Image from dbo.Liushutong where Character = @traditional and image is not null order by LiushutongId;";
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
                                    OldKai = reader.ToNullableAndTrim(nameof(Models.Etymology.OldKai)),
                                    Variants796_810 = reader.ToNullableAndTrim(nameof(Models.Etymology.Variants796_810)),
                                    VariantMeaning = reader.ToNullableAndTrim(nameof(Models.Etymology.VariantMeaning)),
                                    SimplificationRule = reader.ToNullableAndTrim(nameof(Models.Etymology.SimplificationRule)),
                                    SimplificationNewOld = reader.ToNullableAndTrim(nameof(Models.Etymology.SimplificationNewOld)),
                                    RuleBase1753 = reader.ToNullableAndTrim(nameof(Models.Etymology.RuleBase1753)),
                                    RuleBaseObserved = reader.ToNullableAndTrim(nameof(Models.Etymology.RuleBaseObserved)),
                                    Decomposition = reader.ToNullableAndTrim(nameof(Models.Etymology)),
                                    Meaning = reader.ToNullableAndTrim(nameof(Models.Etymology.Meaning)),
                                    CompoundExample = reader.ToNullableAndTrim(nameof(Models.Etymology.CompoundExample)),
                                    SimplifiedCharacterNumber = reader.ToNullableAndTrim(Models.Etymology.SimplifiedCharacterNumberColumn),
                                    EnglishMeanings = reader.ToNullableAndTrim(nameof(Models.Etymology.EnglishMeanings)),
                                    LiuShuStandardName = reader.ToNullableAndTrim(nameof(Models.Etymology.LiuShuStandardName)),
                                    MeaningClass = reader.ToNullableAndTrim(nameof(Models.Etymology.MeaningClass)),
                                    PictographName = reader.ToNullableAndTrim(nameof(Models.Etymology.PictographName)),
                                    Pinyin1 = reader.ToNullableAndTrim(nameof(Models.Etymology.Pinyin1)),
                                    PinyinN = reader.ToNullableAndTrim(nameof(Models.Etymology.PinyinN))
                                });
                            }
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    oracles.Add(new Oracle()
                                    {
                                        OracleId = (int)reader[nameof(Models.Oracle.OracleId)],
                                        Image = (byte[])reader[nameof(Models.Oracle.Image)]
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
                                        Image = (byte[])reader[nameof(Models.Bronze.Image)]
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
                                        Image = (byte[])reader[nameof(Models.Seal.Image)]
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
                                        Image = (byte[])reader[nameof(Models.Liushutong.Image)]
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
            this.Bronze.Where(bonze => bonze.Image != null);

        public IQueryable<Liushutong> LiushutongImages() =>
            this.Liushutong.Where(liushutong => liushutong.Image != null);

        public IQueryable<Oracle> OracleImages() =>
            this.Oracle.Where(oracle => oracle.Image != null);

        public IQueryable<Seal> SealImages() =>
            this.Seal.Where(seal => seal.Image != null);
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
