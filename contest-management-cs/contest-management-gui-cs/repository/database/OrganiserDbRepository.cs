using System;
using System.Collections.Generic;
using System.Data.SQLite;
using contest_management_gui_cs.model;
using contest_management_gui_cs.utils;
using log4net;

namespace contest_management_gui_cs.repository
{
    public class OrganiserDbRepository : IOrganiserRepository
    {
        private readonly DbUtils _dbUtils;

        private static readonly ILog Log = LogManager.GetLogger("mylogger");

        public OrganiserDbRepository(string connectionName)
        {
            _dbUtils = new DbUtils(connectionName);
        }

        public IEnumerable<Organiser> FindAll()
        {
            Log.TraceEntry();
            
            var conn = _dbUtils.GetConnection();
            
            using var cmd = new SQLiteCommand("select * from Organisers", conn);
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
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return new List<Organiser>();
        }

        public Organiser Find(string id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"select * from Organisers
                 where username = @username",
                conn);
            cmd.Parameters.AddWithValue("@username", id);
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
                Log.Error(e);
                Console.WriteLine("Error DB" + e);
            }
            finally
            {
                Log.TraceExit();
            }

            return default;
        }

        public void Add(Organiser entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"insert into Organisers (username, password, first_name, last_name) VALUES
                (@username, @password,@first_name, @last_name)",
                conn);
            cmd.Parameters.AddWithValue("@username", entity.Username);
            cmd.Parameters.AddWithValue("@password", entity.Password);
            cmd.Parameters.AddWithValue("@first_name", entity.FirstName);
            cmd.Parameters.AddWithValue("@last_name", entity.LastName);
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

        public void Remove(string id)
        {
            Log.TraceEntry();

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand("delete from Organisers where username = @username", conn);
            cmd.Parameters.AddWithValue("@username", id);
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

        public void Update(Organiser entity)
        {
            Log.TraceEntry(entity.ToString());

            var conn = _dbUtils.GetConnection();

            using var cmd = new SQLiteCommand(
                @"update Organisers set
                password = @password,
                first_name = @first_name,
                last_name = @last_name
                where username = @username",
                conn);
            cmd.Parameters.AddWithValue("@password", entity.Password);
            cmd.Parameters.AddWithValue("@first_name", entity.FirstName);
            cmd.Parameters.AddWithValue("@last_name", entity.LastName);
            cmd.Parameters.AddWithValue("@username", entity.Username);
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
    }
}