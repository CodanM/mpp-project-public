using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using model;
using persistence.utils;

namespace persistence.database
{
    public class CompetitionDbRepository : ICompetitionRepository
    {
        public IEnumerable<Competition> FindAll()
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();
            
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from Competitions";
            try
            {
                using var reader = cmd.ExecuteReader();
                var competitions = new List<Competition>();
                while (reader.Read())
                {
                    competitions.Add(new Competition
                    {
                        Id = reader.GetInt64(0),
                        CompetitionType = reader.GetString(1),
                        AgeCategory = reader.GetString(2)
                    });
                }

                return competitions;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return new List<Competition>();
        }

        public Competition? Find(long id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();
            
            using var cmd = conn.CreateCommand();
            cmd.CommandText = 
                @"select * from Competitions
                 where id = @id";
            cmd.AddParameterWithValue("@id", id);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Competition
                    {
                        Id = reader.GetInt64(0),
                        CompetitionType = reader.GetString(1),
                        AgeCategory = reader.GetString(2),
                    };
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }
            
            return default;
        }

        public void Add(Competition entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = 
                @"insert into Competitions (competition_type, age_category) VALUES
                (@competition_type, @age_category)";
            cmd.AddParameterWithValue("@competition_type", entity.CompetitionType);
            cmd.AddParameterWithValue("@age_category", entity.AgeCategory);
            try
            {
                entity.Id = (long) (cmd.ExecuteScalar() ?? throw new NullReferenceException());
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public void Remove(long id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "delete from Competitions where id = @id";
            cmd.AddParameterWithValue("@id", id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public void Update(Competition entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"update Competitions set
                competition_type = @competition_type,
                age_category = @age_category
                where id = @id";
            cmd.AddParameterWithValue("@competition_type", entity.CompetitionType);
            cmd.AddParameterWithValue("@age_category", entity.AgeCategory);
            cmd.AddParameterWithValue("@id", entity.Id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public IDictionary<long, (Competition, int)> GetCompetitionsAndCounts(string competitionTypeStr,
            string ageCategoryStr)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select id, competition_type, age_category, count(competition_id) cnt from Competitions C
                left join Registrations R on C.id = R.competition_id
                where
                competition_type like '%' || @competition_type || '%' and
                age_category like '%' || @age_category || '%'
                group by id, competition_id, competition_type, age_category";
            cmd.AddParameterWithValue("@competition_type", competitionTypeStr);
            cmd.AddParameterWithValue("@age_category", ageCategoryStr);
            try
            {
                using var reader = cmd.ExecuteReader();
                var dict = new Dictionary<long, (Competition, int)>();
                while (reader.Read())
                {
                    var id = reader.GetInt64(0);
                    dict.Add(id,
                    (
                        new Competition
                        {
                            Id = id,
                            CompetitionType = reader.GetString(1),
                            AgeCategory = reader.GetString(2)
                        },
                        reader.GetInt32(3)
                    ));
                }

                return dict;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }

            return new Dictionary<long, (Competition, int)>();
        }

        public IEnumerable<string> FindCompetitionTypes(string competitionTypeStr)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select distinct competition_type from Competitions
                where
                competition_type like '%' || @competition_type || '%'";
            cmd.AddParameterWithValue("@competition_type", competitionTypeStr);
            try
            {
                using var reader = cmd.ExecuteReader();
                var competitionTypes = new List<string>();
                while (reader.Read())
                {
                    competitionTypes.Add(reader.GetString(0));
                }

                return competitionTypes;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return new List<string>();
        }

        public IEnumerable<string> FindAgeCategoriesFromCompetitionType(string competitionType)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select age_category from Competitions
                where competition_type = @competition_type";
            cmd.AddParameterWithValue("@competition_type", competitionType);
            try
            {
                using var reader = cmd.ExecuteReader();
                var ageCategories = new List<string>();
                while (reader.Read())
                    ageCategories.Add(reader.GetString(0));
                return ageCategories;
            }
            catch (DbException e)
            {
                Console.WriteLine("Error DB " + e);
            }

            return new List<string>();
        }

        public Competition? FindByProps(string competitionType, string ageCategory)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select * from Competitions
                where competition_type = @competition_type and age_category = @age_category";
            cmd.AddParameterWithValue("@competition_type", competitionType);
            cmd.AddParameterWithValue("@age_category", ageCategory);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Competition
                    {
                        Id = reader.GetInt64(0),
                        CompetitionType = reader.GetString(1),
                        AgeCategory = reader.GetString(2)
                    };
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return default;
        }
    }
}