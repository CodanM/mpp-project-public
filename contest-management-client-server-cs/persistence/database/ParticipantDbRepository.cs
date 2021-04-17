using System;
using System.Collections.Generic;
using System.Data.SQLite;
using model;
using persistence.utils;

namespace persistence.database
{
    public class ParticipantDbRepository : IParticipantRepository
    {

        public IEnumerable<Participant> FindAll()
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Participants";
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
                Console.WriteLine("Error DB" + e);
            }

            return new List<Participant>();
        }

        public Participant? Find(long id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select * from Participants
                 where id = @id";
            cmd.AddParameterWithValue("@id", id);
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
                Console.WriteLine("Error DB" + e);
            }

            return default;
        }

        public void Add(Participant entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"insert into Participants (first_name, last_name, age) VALUES
                (@first_name, @last_name, @age);
                select last_insert_rowid();";
            cmd.AddParameterWithValue("@first_name", entity.FirstName);
            cmd.AddParameterWithValue("@last_name", entity.LastName);
            cmd.AddParameterWithValue("@age", entity.Age);
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
            cmd.CommandText = "delete from Participants where id = @id";
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

        public void Update(Participant entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"update Participants set
                first_name = @first_name,
                last_name = @last_name,
                age = @age
                where id = @id";
            cmd.AddParameterWithValue("@first_name", entity.FirstName);
            cmd.AddParameterWithValue("@last_name", entity.LastName);
            cmd.AddParameterWithValue("@age", entity.Age);
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

        public IEnumerable<Participant> FindParticipantsByCompetition(long competitionId)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select participant_id, first_name, last_name, age from Participants P
                inner join Registrations R on P.id = R.participant_id
                where competition_id = @competition_id";
            cmd.AddParameterWithValue("@competition_id", competitionId);
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
                Console.WriteLine("Error DB " + e);
            }

            return new List<Participant>();
        }

        public IEnumerable<long> FindParticipantIdsByName(string firstName, string lastName)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select id from Participants
                 where first_name = @first_name and last_name = @last_name";
            cmd.AddParameterWithValue("@first_name", firstName);
            cmd.AddParameterWithValue("@last_name", lastName);
            try
            {
                List<long> ids = new List<long>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    ids.Add(reader.GetInt64(0));

                return ids;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return new List<long>();
        }
    }
}