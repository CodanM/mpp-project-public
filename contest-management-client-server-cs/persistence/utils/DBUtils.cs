using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace persistence.utils
{
    public static class DbUtils
    {
        private static IDbConnection? _instance;

        public static IDbConnection GetConnection()
        {
            //if (_instance != null  && _instance.State != ConnectionState.Closed) return _instance;
            
            _instance = GetNewConnection();
            return _instance;
        }

        private static IDbConnection GetNewConnection()
        {
            return ConnectionFactory.GetInstance()?.CreateConnection() ?? throw new NullReferenceException();
        }
    }

    public abstract class ConnectionFactory
    {
        private static ConnectionFactory? _instance;

        public static ConnectionFactory? GetInstance()
        {
            if (_instance != null) return _instance;

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(ConnectionFactory)))
                    _instance = (ConnectionFactory?) Activator.CreateInstance(type);
            }

            return _instance;
        }

        public abstract IDbConnection CreateConnection();
    }

    // ReSharper disable once InconsistentNaming
    public class SQLiteConnectionFactory : ConnectionFactory
    {
        public override IDbConnection CreateConnection()
        {
            Console.WriteLine("Creating SQLite connection...");
            var connectionString = ConfigurationManager.ConnectionStrings["contestmgmt-db"]?.ConnectionString;
            return new SQLiteConnection(connectionString);
        }
    }
}