using Bilim_Drop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilim_Drop
{
    public interface Repository
    {
        Task<Quiz[]> getQuizzes();
        Task<int> insertOrUpdateQuiz(Quiz arg);
        Task<int> deleteQuiz(Quiz arg);
    }
    class RepositoryImpl : Repository
    {
        private Sql sql = new Sql();

        public Task<int> deleteQuiz(Quiz arg)
        {
            throw new NotImplementedException();
        }

        public Task<Quiz[]> getQuizzes()
        {
            var list = new List<Quiz>();
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"SELECT * FROM quizzes";
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

        public Task<int> insertOrUpdateQuiz(Quiz arg)
        {
            throw new NotImplementedException();
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
