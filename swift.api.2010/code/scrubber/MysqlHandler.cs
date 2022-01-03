using log4net;
using MySql.Data.MySqlClient;
using System;

namespace swift.api._2010.code.scrubber
{
    public class MysqlHandler : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MysqlHandler));

        readonly object locker = new object();
        readonly string connectionStringForApi = "Server=95.216.10.237;Port=3306;Database=spamtraps;Uid=jarvis;Pwd=CrypticP001!;Pooling=True;default command timeout=60;Connection Timeout=60;";

        protected MySqlConnection ConnectionForApi { get; set; }

        protected MySqlConnection GetConnectionForApi()
        {
            if (ConnectionForApi != null && ConnectionForApi.State == System.Data.ConnectionState.Open)
            {
                return ConnectionForApi;
            }

            ConnectionForApi = new MySqlConnection(connectionStringForApi);
            ConnectionForApi.Open();

            return ConnectionForApi;
        }

        public int ValidateF(string apiKey, string c1, string c2)
        {
            MySqlCommand cmd = GetConnectionForApi().CreateCommand();
            cmd.CommandText = string.Format("select validate ('{0}','{1}','{2}') as qt", apiKey, c1, c2);

            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int qt = reader.GetInt32("qt");

                        reader.Close();

                        return qt;
                    }

                    return -1;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return 6;
        }

        internal void CloseConnection(MySqlConnection connection)
        {
            try
            {
                if (connection == null)
                {
                    return;
                }
                
                connection.Close();
            }
            catch { }
        }

        internal void CloseConnections()
        {
            CloseConnection(ConnectionForApi);
        }

        public void Dispose()
        {
            CloseConnections();
        }
    }
}