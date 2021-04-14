using System.Data;

namespace persistence.utils
{
    public static class DbCommandExtensionMethods
    {
        public static void AddParameterWithValue<T>(this IDbCommand cmd, string parameterName, T value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}