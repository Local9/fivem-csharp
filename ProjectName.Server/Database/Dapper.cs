using Dapper;
using Logger;
using MySqlConnector;

namespace ProjectName.Server.Database
{
    internal class Dapper<T>
    {
        public static async Coroutine<List<T>> GetListAsync(string query, DynamicParameters args = null)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfiguration.ConnectionString()))
                {
                    SetupTypeMap();

                    return (await conn.QueryAsync<T>(query, args)).AsList();
                }
            }
            catch (Exception ex)
            {
                SqlExceptionHandler(query, args, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }
            return null;
        }

        public static async Coroutine<T> GetSingleAsync(string query, DynamicParameters args = null)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfiguration.ConnectionString()))
                {
                    SetupTypeMap();
                    return (await conn.QueryAsync<T>(query, args)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                SqlExceptionHandler(query, args, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }
            return default(T);
        }

        public static async Coroutine<bool> ExecuteAsync(string query, DynamicParameters args = null)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                using (var conn = new MySqlConnection(DatabaseConfiguration.ConnectionString()))
                {
                    return (await conn.ExecuteAsync(query, args)) > 0;
                }
            }
            catch (Exception ex)
            {
                SqlExceptionHandler(query, args, ex.Message, watch.ElapsedMilliseconds);
            }
            finally
            {
                watch.Stop();
            }
            return false;
        }

        private static void SqlExceptionHandler(string query, DynamicParameters args, string exceptionMessage, long elapsedMilliseconds)
        {
            StringBuilder sb = new();
            sb.Append("** SQL Exception **\n");
            sb.Append($"Query: {query}\n");
            foreach (var arg in args.ParameterNames)
            {
                sb.Append($"Parameter: {arg} Value: {args.Get<object>(arg)}\n");
            }
            sb.Append($"Exception Message: {exceptionMessage}\n");
            sb.Append($"Time Elapsed: {elapsedMilliseconds}ms");
            Main.Logger.Error($"{LoggerColors.DARK_RED}{sb}");
        }

        private static void SetupTypeMap()
        {
            var map = new CustomPropertyTypeMap(typeof(T), (type, columnName) =>
                                type.GetProperties().FirstOrDefault(prop => GetDescriptionFromAttribute(prop) == columnName.ToLower()));
            SqlMapper.SetTypeMap(typeof(T), map);
        }

        public static string GetDescriptionFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (DescriptionAttribute)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false);
            return (attrib?.Description ?? member.Name).ToLower();
        }
    }
}
