using System;
using System.Data.SQLite;

namespace Bilim_Drop
{
    public class Sql
    {
        private string connectionString = "Data Source=bilimdrop_database.db";
        public void UseCmd(Action<SQLiteCommand> action)
        {
            using (var conn = new SQLiteConnection(connectionString))
            using (var cmd = new SQLiteCommand())
            {
                cmd.Connection = conn;
                conn.Open();
                action(cmd);
            }
        }
        public void UseReader(SQLiteCommand cmd, Action<SQLiteDataReader> action)
        {
            using (SQLiteDataReader rd = cmd.ExecuteReader())
            {
                if (rd.HasRows) action(rd);
                rd.Close();
            }
        }
        public string ReplaceBetweenWords(string input, string startWord, string endWord, string newValue)
        {
            int startIndex = input.IndexOf(startWord) + startWord.Length;
            int endIndex = input.IndexOf(endWord, startIndex);
            if (startIndex < 0 || endIndex < 0)
            {
                return input;
            }
            string partBefore = input.Substring(0, startIndex);
            string partAfter = input.Substring(endIndex);
            string replacedString = partBefore + newValue + partAfter;
            return replacedString;
        }
    }
}
