using System;
using System.Collections.Generic;
using System.Data.SQLite;
using model;
using persistence.utils;

namespace persistence.database
{
    public class RegistrationDbRepository : IRegistrationRepository
    {
        public IEnumerable<Registration> FindAll()
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select * from Registrations R
                inner join Participants P on P.id = R.participant_id
                inner join Competitions C on C.id = R.competition_id";
            try
            {
                using var reader = cmd.ExecuteReader();
                var registrations = new List<Registration>();
                while (reader.Read())
                {
                    registrations.Add(new Registration
                    (
                        new Participant
                        {
                            Id = reader.GetInt64(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            Age = reader.GetInt32(5)
                        },
                        new Competition
                        {
                            Id = reader.GetInt64(6),
                            CompetitionType = reader.GetString(7),
                            AgeCategory = reader.GetString(8)
                        }
                    ));
                }

                return registrations;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return new List<Registration>();
        }

        public Registration? Find((long, long) id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select * from Registrations R
                inner join Participants P on P.id = R.participant_id
                inner join Competitions C on C.id = R.competition_id
                where P.id = @participant_id and C.id = @competition_id";
            var (participantId, competitionId) = id;
            cmd.AddParameterWithValue("@participant_id", participantId);
            cmd.AddParameterWithValue("@competition_id", competitionId);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Registration
                    (
                        new Participant
                        {
                            Id = reader.GetInt64(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            Age = reader.GetInt32(5)
                        },
                        new Competition
                        {
                            Id = reader.GetInt64(6),
                            CompetitionType = reader.GetString(7),
                            AgeCategory = reader.GetString(8)
                        }
                    );
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return default;
        }

        public void Add(Registration entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"insert into Registrations (participant_id, competition_id) VALUES
                (@participant_id, @competition_id)";
            cmd.AddParameterWithValue("@participant_id", entity.Participant.Id);
            cmd.AddParameterWithValue("@competition_id", entity.Competition.Id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public void Remove((long, long) id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"delete from Registrations
                where participant_id = @participant_id and competition_id = @competition_id";
            var (participantId, competitionId) = id;
            cmd.AddParameterWithValue("@participant_id", participantId);
            cmd.AddParameterWithValue("@competition_id", competitionId);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public void Update(Registration entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Registration> FindByParticipant(long participantId)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select * from Registrations R
                inner join Participants P on P.id = R.participant_id
                inner join Competitions C on C.id = R.competition_id
                where P.id = @participant_id";
            cmd.AddParameterWithValue("@participant_id", participantId);
            try
            {
                using var reader = cmd.ExecuteReader();
                var registrations = new List<Registration>();
                while (reader.Read())
                    registrations.Add(new Registration
                    (
                        new Participant
                        {
                            Id = reader.GetInt64(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            Age = reader.GetInt32(5)
                        },
                        new Competition
                        {
                            Id = reader.GetInt64(6),
                            CompetitionType = reader.GetString(7),
                            AgeCategory = reader.GetString(8)
                        }
                    ));
                
                return registrations;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return new List<Registration>();
        }
    }
}