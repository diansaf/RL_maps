using MySql.Data.MySqlClient;

namespace maps
{
    public class ConnectionMysql
    {
        public MySqlConnection GetConnection()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mysqlConnection"].ToString();
            return new MySqlConnection(connectionString);
        }
    }
}