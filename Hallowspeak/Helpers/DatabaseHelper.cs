using Hallowspeak.Data.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallowspeak.Helpers
{
    /// <summary>
    /// Contains Reusable functions for Database Operations.
    /// </summary>
    public class DatabaseHelper
    {

        private readonly string _server, _user, _pass;
        /// <summary>
        /// If no Database Credentials are available, this bool can be checked to make sure no unnecessary slowdowns/errors are produced.
        /// </summary>
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

        /// <summary>
        /// Get a Closed DB connection.
        /// </summary>
        /// <returns>A Copy of a Closed DB Connection</returns>
        public MySqlConnection GetConnection()
        {
            string connStr = $"Server={_server};Uid={_user};Database=Hallowspeak;port=3306;Password={_pass};SslMode=none;CharSet=utf8mb4;Connect Timeout=1";
            MySqlConnection conn = new MySqlConnection(connStr);
            return conn;
        }

        /// <summary>
        /// Pull all Lexicon Data from the DB.
        /// </summary>
        public async Task<List<LexiconItem>> GetLexiconItemsFromDB(MySqlConnection conn)
        {
            List<LexiconItem> output = new List<LexiconItem>();
            try
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Lexicon`", conn);

                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        LexiconItem item = new LexiconItem(reader.GetInt32(0));
                        for (int i = 1; i < reader.FieldCount; i++)
                        {
                            item.KeyValues.Add(reader.GetName(i), new LexiconValue(reader.GetString(i)));
                        }
                        output.Add(item);
                    }
                }
            }
            catch 
            {

            }
            finally
            {
                await conn.CloseAsync();
            }
            return output;
        }

        public async Task<List<string>> GetTableNames(MySqlConnection conn)
        {
            List<string> output = new List<string>();

            try
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand("SHOW tables", conn);

                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        output.Add(reader.GetString(0));
                    }
                }
            }
            catch {}
            finally
            {
                await conn.CloseAsync();
            }
            return output;
        }

        public async Task<List<Dictionary<string, object>>> GetTable(MySqlConnection conn, string tableName)
        {
            List<Dictionary<string, object>> output = new List<Dictionary<string, object>>();

            try
            {
                await conn.OpenAsync();

                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM `{tableName}`", conn);

                using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Dictionary<string, object> tmp = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            tmp.Add(reader.GetName(i), reader.GetValue(i));
                        }

                        output.Add(tmp);
                    }
                }
            }
            catch { }
            finally
            {
                await conn.CloseAsync();
            }
            return output;
        }
    }
}
