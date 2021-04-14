using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using log4net;

namespace contest_management_gui_cs.utils
{
    public class DbUtils
    {
        private static readonly ILog Log =
            LogManager.GetLogger("mylogger");

        private readonly string _connectionString;

        public DbUtils(string connectionName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionName]?.ConnectionString;
        }

        private static SQLiteConnection _instance = null;
        
        private SQLiteConnection GetNewConnection()
        {
            Log.TraceEntry();
            return new SQLiteConnection(_connectionString);
        }

        public SQLiteConnection GetConnection()
        {
            Log.TraceEntry();
            if (_instance != null && _instance.State != ConnectionState.Closed) return _instance;
            _instance = GetNewConnection();
            _instance.Open();
            Log.TraceExit();
            return _instance;
        }
    }
}