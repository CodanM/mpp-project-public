using System;
using System.Collections.Generic;
using System.Data.SQLite;
using contest_management_gui_cs.model;
using contest_management_gui_cs.utils;
using log4net;

namespace contest_management_gui_cs.repository
{
    public class ParticipantDbRepository : IParticipantRepository
    {
        private readonly DbUtils _dbUtils;

        private static readonly ILog Log = LogManager.GetLogger("mylogger");
        
        public ParticipantDbRepository(string connectionName)
        {
            _dbUtils = new DbUtils(connectionName);
        }
        
        public IEnumerable<Participant> FindAll()
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand("select * from Participants", conn);
            try
            {
                using var reader = cmd.ExecuteReader();
                var participants = new List<Participant>();
                while (reader.Read())
                {
                    participants.Add(new Participant
                    {
                        Id = reader.GetInt64(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Age = reader.GetInt32(3)
                    });
                }

                return participants;
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

            return new List<Participant>();
        }

        public Participant Find(long id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Participants
                 where id = @id",
                conn);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Participant
                    {
                        Id = reader.GetInt64(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Age = reader.GetInt32(3)
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

        public void Add(Participant entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"insert into Participants (first_name, last_name, age) VALUES
                (@first_name, @last_name, @age)",
                conn);
            cmd.Parameters.AddWithValue("@first_name", entity.FirstName);
            cmd.Parameters.AddWithValue("@last_name", entity.LastName);
            cmd.Parameters.AddWithValue("@age", entity.Age);
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

            using var cmd = new SQLiteCommand("delete from Participants where id = @id", conn);
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

        public void Update(Participant entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"update Participants set
                first_name = @first_name,
                last_name = @last_name,
                age = @age
                where id = @id",
                conn);
            cmd.Parameters.AddWithValue("@first_name", entity.FirstName);
            cmd.Parameters.AddWithValue("@last_name", entity.LastName);
            cmd.Parameters.AddWithValue("@age", entity.Age);
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

        public IEnumerable<Participant> FindParticipantsByCompetition(long competitionId)
        {
            Log.TraceEntry(competitionId.ToString());
            
            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select participant_id, first_name, last_name, age from Participants P
                inner join Registrations R on P.id = R.participant_id
                where competition_id = @competition_id",
                conn);
            cmd.Parameters.AddWithValue("@competition_id", competitionId);
            try
            {
                using var reader = cmd.ExecuteReader();
                var participants = new List<Participant>();
                while (reader.Read())
                {
                    participants.Add(new Participant
                    {
                        Id = reader.GetInt64(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Age = reader.GetInt32(3)
                    });
                }

                return participants;
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

            return new List<Participant>();
        }

        public Participant FindParticipantByName(string firstName, string lastName)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Participants
                 where first_name = @first_name and last_name = @last_name",
                conn);
            cmd.Parameters.AddWithValue("@first_name", firstName);
            cmd.Parameters.AddWithValue("@last_name", lastName);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Participant
                    {
                        Id = reader.GetInt64(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Age = reader.GetInt32(3)
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