using System;
using System.Collections.Generic;
using System.Data.SQLite;
using model;
using persistence.utils;

namespace persistence.database
{
    public class OrganiserDbRepository : IOrganiserRepository
    {
        public IEnumerable<Organiser> FindAll()
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();
            
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from Organisers";
            
            try
            {
                using var reader = cmd.ExecuteReader();
                var organisers = new List<Organiser>();
                while (reader.Read())
                {
                    organisers.Add(new Organiser
                    {
                        Username = reader.GetString(0),
                        Password = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3)
                    });
                }

                return organisers;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return new List<Organiser>();
        }

        public Organiser? Find(string id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"select * from Organisers
                 where username = @username";
            cmd.AddParameterWithValue("@username", id);
            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new Organiser
                    {
                        Username = reader.GetString(0),
                        Password = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                    };
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB" + e);
            }

            return default;
        }

        public void Add(Organiser entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"insert into Organisers (username, password, first_name, last_name) VALUES
                (@username, @password,@first_name, @last_name)";
            cmd.AddParameterWithValue("@username", entity.Username);
            cmd.AddParameterWithValue("@password", entity.Password);
            cmd.AddParameterWithValue("@first_name", entity.FirstName);
            cmd.AddParameterWithValue("@last_name", entity.LastName);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public void Remove(string id)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "delete from Organisers where username = @username";
            cmd.AddParameterWithValue("@username", id);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }

        public void Update(Organiser entity)
        {
            using var conn = DbUtils.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"update Organisers set
                password = @password,
                first_name = @first_name,
                last_name = @last_name
                where username = @username";
            cmd.AddParameterWithValue("@password", entity.Password);
            cmd.AddParameterWithValue("@first_name", entity.FirstName);
            cmd.AddParameterWithValue("@last_name", entity.LastName);
            cmd.AddParameterWithValue("@username", entity.Username);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("Error DB " + e);
            }
        }
    }
}