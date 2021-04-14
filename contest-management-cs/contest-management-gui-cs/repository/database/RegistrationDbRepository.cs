using System;
using System.Collections.Generic;
using System.Data.SQLite;
using contest_management_gui_cs.model;
using contest_management_gui_cs.utils;
using log4net;

namespace contest_management_gui_cs.repository
{
    public class RegistrationDbRepository : IRegistrationRepository
    {
        private readonly DbUtils _dbUtils;
        
        private static readonly ILog Log = LogManager.GetLogger("mylogger");

        public RegistrationDbRepository(string connectionName)
        {
            _dbUtils = new DbUtils(connectionName);
        }
        
        public IEnumerable<Registration> FindAll()
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Registrations R
                inner join Participants P on P.id = R.participant_id
                inner join Competitions C on C.id = R.competition_id", conn);
            try
            {
                using var reader = cmd.ExecuteReader();
                var registrations = new List<Registration>();
                while (reader.Read())
                {
                    registrations.Add(new Registration
                    {
                        RegisteredParticipant = new Participant
                        {
                            Id = reader.GetInt64(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            Age = reader.GetInt32(5)
                        },
                        WhichCompetition = new Competition
                        {
                            Id = reader.GetInt64(6),
                            CompetitionType = reader.GetString(7),
                            AgeCategory = reader.GetString(8)
                        }
                    });
                }

                return registrations;
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

            return new List<Registration>();
        }

        public Registration Find(Tuple<long, long> id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Registrations R
                inner join Participants P on P.id = R.participant_id
                inner join Competitions C on C.id = R.competition_id
                where P.id = @participant_id and C.id = @competition_id",
                conn);
            cmd.Parameters.AddWithValue("@participant_id", id.Item1);
            cmd.Parameters.AddWithValue("@competition_id", id.Item2);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Registration
                    {
                        RegisteredParticipant = new Participant
                        {
                            Id = reader.GetInt64(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            Age = reader.GetInt32(5)
                        },
                        WhichCompetition = new Competition
                        {
                            Id = reader.GetInt64(6),
                            CompetitionType = reader.GetString(7),
                            AgeCategory = reader.GetString(8)
                        }
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

        public void Add(Registration entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"insert into Registrations (participant_id, competition_id) VALUES
                (@participant_id, @competition_id)",
                conn);
            cmd.Parameters.AddWithValue("@participant_id", entity.RegisteredParticipant.Id);
            cmd.Parameters.AddWithValue("@competition_id", entity.WhichCompetition.Id);
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

        public void Remove(Tuple<long, long> id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"delete from Registrations
                where participant_id = @participant_id and competition_id = @competition_id", conn);
            cmd.Parameters.AddWithValue("@participant_id", id.Item1);
            cmd.Parameters.AddWithValue("@competition_id", id.Item2);
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

        public void Update(Registration entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Registration> FindByParticipant(long participantId)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Registrations R
                inner join Participants P on P.id = R.participant_id
                inner join Competitions C on C.id = R.competition_id
                where P.id = @participant_id",
                conn);
            cmd.Parameters.AddWithValue("@participant_id", participantId);
            try
            {
                using var reader = cmd.ExecuteReader();
                var registrations = new List<Registration>();
                while (reader.Read())
                    registrations.Add(new Registration
                    {
                        RegisteredParticipant = new Participant
                        {
                            Id = reader.GetInt64(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            Age = reader.GetInt32(5)
                        },
                        WhichCompetition = new Competition
                        {
                            Id = reader.GetInt64(6),
                            CompetitionType = reader.GetString(7),
                            AgeCategory = reader.GetString(8)
                        }
                    });
                
                return registrations;
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