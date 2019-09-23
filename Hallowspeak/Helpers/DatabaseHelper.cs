using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallowspeak.Helpers
{
    public class DatabaseHelper
    {
        
        private readonly string _server, _user, _pass;
        public readonly bool Enabled;

#nullable enable
        public DatabaseHelper(string? server, string? user, string? pass)
        {
            if (server == null || user == null || pass == null)
            {
                Enabled = false;
            }
            else
            {
                Enabled = true;
                _server = server;
                _user = user;
                _pass = pass;
            }
        }
#nullable disable

        public MySqlConnection GetConnection()
        {
            string connStr = $"Server={_server};Uid={_user};Database=Hallowspeak;port=3306;Password={_pass};SslMode=none;CharSet=utf8mb4;Connect Timeout=1";
            MySqlConnection conn = new MySqlConnection(connStr);
            return conn;
        }
    }
}
