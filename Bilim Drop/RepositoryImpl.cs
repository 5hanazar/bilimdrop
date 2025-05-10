using Bilim_Drop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilim_Drop
{
    public interface Repository
    {
        Task<Quiz[]> getQuizzes();
        Task<Quiz> getQuiz(int id);
        Task<int> insertOrUpdateQuiz(PostQuiz arg);
        Task<int> deleteQuiz(Quiz arg);
    }
    class RepositoryImpl : Repository
    {
        private Sql sql = new Sql();

        public Task<Quiz[]> getQuizzes()
        {
            var list = new List<Quiz>();
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"SELECT id, active, title, description, createdGmt FROM quizzes";
                sql.UseReader(cmd, rd =>
                {
                    while (rd.Read())
                    {
                        list.Add(new Quiz(rd.GetInt32(0), rd.GetInt32(1) == 1, rd.GetString(2), rd.GetString(3), GmtToString(rd.GetInt32(4))));
                    };
                });
            });
            return Task.FromResult(list.ToArray());
        }
        public Task<Quiz> getQuiz(int id)
        {
            Quiz result = null;
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"SELECT id, active, title, description, createdGmt FROM quizzes WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();
                sql.UseReader(cmd, rd =>
                {
                    while (rd.Read())
                    {
                        result = new Quiz(rd.GetInt32(0), rd.GetInt32(1) == 1, rd.GetString(2), rd.GetString(3), GmtToString(rd.GetInt32(4)));
                    }
                });
            });
            return Task.FromResult(result);
        }
        public Task<int> insertOrUpdateQuiz(PostQuiz arg)
        {
            int id = arg.id;
            sql.UseCmd(cmd =>
            {
                int secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                if (id == 0)
                {
                    cmd.CommandText = "INSERT INTO quizzes(active, title, description, createdGmt) VALUES(@active, @title, @description, @createdGmt); SELECT id FROM quizzes ORDER BY id DESC LIMIT 1";
                    cmd.Parameters.AddWithValue("@createdGmt", secondsSinceEpoch);
                }
                else
                {
                    cmd.CommandText = "UPDATE quizzes SET active = @active, title = @title, description = @description WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id).Value = id;
                };
                cmd.Parameters.AddWithValue("@active", arg.active ? 1 : 0);
                cmd.Parameters.AddWithValue("@title", arg.title);
                cmd.Parameters.AddWithValue("@description", arg.description);
                cmd.Prepare();

                if (id == 0) id = Convert.ToInt32(cmd.ExecuteScalar());
                else cmd.ExecuteNonQuery();
            });
            return Task.FromResult(id);
        }
        public Task<int> deleteQuiz(Quiz arg)
        {
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = "DELETE FROM quizzes WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", arg.id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            });
            return Task.FromResult(arg.id);
        }

        private string GmtToString(int seconds)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateTime = unixEpoch.AddSeconds(seconds);
            return dateTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
        }
        private float parse(string arg)
        {
            return float.TryParse(arg, out var v) ? (float)Math.Round(v, 2) : 0f;
        }
    }
}
