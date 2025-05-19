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
                        list.Add(new Quiz(rd.GetInt32(0), rd.GetInt32(1) == 1, rd.GetString(2), rd.GetString(3), gmtToString(rd.GetInt32(4))));
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
                        var quizId = rd.GetInt32(0);
                        result = new Quiz(quizId, rd.GetInt32(1) == 1, rd.GetString(2), rd.GetString(3), gmtToString(rd.GetInt32(4)), _getQuestions(quizId));
                    }
                });
            });
            return Task.FromResult(result);
        }
        private Question[] _getQuestions(int quizId)
        {
            var result = new List<Question>();
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"SELECT id, questionType, title FROM questions WHERE quizId = @quizId ORDER BY line";
                cmd.Parameters.AddWithValue("@quizId", quizId);
                cmd.Prepare();
                sql.UseReader(cmd, rd =>
                {
                    while (rd.Read())
                    {
                        var questionId = rd.GetInt32(0);
                        result.Add(new Question(questionId, rd.GetInt32(1), rd.GetString(2), _getAnswers(questionId)));
                    }
                });
            });
            return result.ToArray();
        }
        private Answer[] _getAnswers(int questionId)
        {
            var result = new List<Answer>();
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"SELECT title, isCorrect FROM answers WHERE questionId = @questionId ORDER BY line";
                cmd.Parameters.AddWithValue("@questionId", questionId);
                cmd.Prepare();
                sql.UseReader(cmd, rd =>
                {
                    while (rd.Read())
                    {
                        result.Add(new Answer(rd.GetString(0), rd.GetInt32(1) == 1));
                    }
                });
            });
            return result.ToArray();
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
                else
                {
                    cmd.CommandText += $"; DELETE FROM questions WHERE quizId = {id}; DELETE FROM answers WHERE quizId = {id}";
                    cmd.ExecuteNonQuery();
                }
            });
            for (int i = 0; i < arg.questions.Length; i++)
            {
                _insertQuestion(id, arg.questions[i]);
            }
            return Task.FromResult(id);
        }

        private void _insertQuestion(int quizId, PostQuestion value)
        {
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"INSERT INTO questions(id, quizId, line, questionType, title) values(@id, @quizId, @line, @questionType, @title)";
                cmd.Parameters.AddWithValue("@id", value.id);
                cmd.Parameters.AddWithValue("@quizId", quizId);
                cmd.Parameters.AddWithValue("@line", value.line);
                cmd.Parameters.AddWithValue("@questionType", value.questionType);
                cmd.Parameters.AddWithValue("@title", value.title);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            });
            for (int i = 0; i < value.answers.Length; i++)
            {
                _insertAnswer(quizId, value.id, value.answers[i]);
            }
        }

        private void _insertAnswer(int quizId, int questionId, PostAnswer value)
        {
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = $"INSERT INTO answers(quizId, questionId, line, title, isCorrect) values(@quizId, @questionId, @line, @title, @isCorrect)";
                cmd.Parameters.AddWithValue("@quizId", quizId);
                cmd.Parameters.AddWithValue("@questionId", questionId);
                cmd.Parameters.AddWithValue("@line", value.line);
                cmd.Parameters.AddWithValue("@title", value.title);
                cmd.Parameters.AddWithValue("@isCorrect", value.isCorrect);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            });
        }

        public Task<int> deleteQuiz(Quiz arg)
        {
            sql.UseCmd(cmd =>
            {
                cmd.CommandText = "DELETE FROM quizzes WHERE id = @id; DELETE FROM questions WHERE quizId = @id; DELETE FROM answers WHERE quizId = @id";
                cmd.Parameters.AddWithValue("@id", arg.id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            });
            return Task.FromResult(arg.id);
        }

        private string gmtToString(int seconds)
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
