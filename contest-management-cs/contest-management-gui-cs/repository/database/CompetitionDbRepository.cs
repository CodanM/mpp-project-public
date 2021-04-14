using System;
using System.Collections.Generic;
using System.Data.SQLite;
using contest_management_gui_cs.model;
using contest_management_gui_cs.utils;
using log4net;

namespace contest_management_gui_cs.repository
{
    public class CompetitionDbRepository : ICompetitionRepository
    {
        private readonly DbUtils _dbUtils;
        
        private static readonly ILog Log = LogManager.GetLogger("mylogger");

        public CompetitionDbRepository(string connectionName)
        {
            _dbUtils = new DbUtils(connectionName);
        }
        
        public IEnumerable<Competition> FindAll()
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand("select * from Competitions", conn);
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
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return new List<Competition>();
        }

        public Competition Find(long id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Competitions
                 where id = @id",
                conn);
            cmd.Parameters.AddWithValue("@id", id);
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
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return default;
        }

        public void Add(Competition entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"insert into Competitions (competition_type, age_category) VALUES
                (@competition_type, @age_category)",
                conn);
            cmd.Parameters.AddWithValue("@competition_type", entity.CompetitionType);
            cmd.Parameters.AddWithValue("@age_category", entity.AgeCategory);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Log.Error(e);
                Console.WriteLine("Error DB " + e);
            }
            finally
            {
                Log.TraceExit();
            }
        }

        public void Remove(long id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand("delete from Competitions where id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Log.Error(e);
                Console.WriteLine("Error DB " + e);
            }
            finally
            {
                Log.TraceExit();
            }
        }

        public void Update(Competition entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"update Competitions set
                competition_type = @competition_type,
                age_category = @age_category
                where id = @id",
                conn);
            cmd.Parameters.AddWithValue("@competition_type", entity.CompetitionType);
            cmd.Parameters.AddWithValue("@age_category", entity.AgeCategory);
            cmd.Parameters.AddWithValue("@id", entity.Id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Log.Error(e);
                Console.WriteLine("Error DB " + e);
            }
            finally
            {
                Log.TraceExit();
            }
        }

        public IDictionary<Competition, int> CountParticipantsForEachCompetition()
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select C.id, competition_type, age_category, count(competition_id) cnt from Competitions C
                left join Registrations R on C.id = R.competition_id
                group by competition_id, competition_type, age_category",
                conn);
            try
            {
                using var reader = cmd.ExecuteReader();
                var dict = new Dictionary<Competition, int>();
                while (reader.Read())
                {
                    dict[new Competition
                    {
                        Id = reader.GetInt64(0),
                        CompetitionType = reader.GetString(1),
                        AgeCategory = reader.GetString(2)
                    }] = reader.GetInt32(3);
                }

                return dict;
            }
            catch (SQLiteException e)
            {
                Log.Error(e);
                Console.WriteLine("Error DB " + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return new Dictionary<Competition, int>();
        }

        public IEnumerable<string> FindTypesByString(string competitionType)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select distinct competition_type from Competitions
                where competition_type like '%' || @competition_type || '%'",
                conn);
            cmd.Parameters.AddWithValue("@competition_type", competitionType);
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
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return new List<string>();
        }

        public IEnumerable<string> FindAgeCategories(string competitionType)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select age_category from Competitions
                where competition_type = @competition_type",
                conn);
            cmd.Parameters.AddWithValue("@competition_type", competitionType);
            try
            {
                using var reader = cmd.ExecuteReader();
                var ageCategories = new List<string>();
                while (reader.Read())
                {
                    ageCategories.Add(reader.GetString(0));
                }

                return ageCategories;
            }
            catch (SQLiteException e)
            {
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return new List<string>();
        }

        public Competition FindByProps(string competitionType, string ageCategory)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Competitions
                where competition_type = @competition_type and age_category = @age_category",
                conn);
            cmd.Parameters.AddWithValue("@competition_type", competitionType);
            cmd.Parameters.AddWithValue("@age_category", ageCategory);
            try
            {
                using var reader = cmd.ExecuteReader();
                var competitions = new List<Competition>();
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
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return default;
        }
    }
}