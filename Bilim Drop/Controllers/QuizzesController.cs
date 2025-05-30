using Bilim_Drop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bilim_Drop.Controllers
{
    public class QuizzesController : ApiController
    {
        Repository repo = new RepositoryImpl();
        public async Task<HttpResponseMessage> Get()
        {
            var d = Request.GetQueryNameValuePairs();

            //Id
            var _id = d.Where(nv => nv.Key == "id").Select(nv => nv.Value).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(_id))
            {
                int sub_id = 0;
                var cookie = Request.Headers.GetCookies("sub_id").FirstOrDefault();
                if (cookie == null)
                {
                    var quiz = await repo.getQuiz(int.Parse(_id));
                    var questionList = new List<QuestionView>();
                    Array.ForEach(quiz.questions, (question) =>
                    {
                        var answerList = new List<AnswerView>();
                        Array.ForEach(question.answers, (answer) => {
                            answerList.Add(new AnswerView(answer.line, answer.title));
                        });
                        questionList.Add(new QuestionView(question.id, question.questionType, question.title, answerList.ToArray()));
                    });
                    var quizView = new QuizView(quiz.id, quiz.title, quiz.description, quiz.createdDate, questionList.ToArray());
                    sub_id = await repo.insertOrUpdateSubmission(new Submission(0, false, "steve", int.Parse(_id), JsonConvert.SerializeObject(quizView), "", ""));
                }
                else sub_id = int.Parse(cookie["sub_id"].Value);
                var submission = await repo.getSubmission(sub_id);    
                
                var r = new HttpResponseMessage();
                r.Content = new StringContent(generateHtml(JsonConvert.DeserializeObject<QuizView>(submission.quizJ)));
                var cc = new CookieHeaderValue("sub_id", sub_id.ToString());
                r.Headers.AddCookies(new CookieHeaderValue[] { cc });
                r.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return r;
            }

            var html = File.ReadAllText("html/quizzes.html");
            var aLinks = "";
            var quizzes = await repo.getQuizzes();
            if (quizzes.Length > 0)
            {
                Array.ForEach(quizzes, e =>
                {
                    aLinks += $"<a href=\"html/quiz.html\" class=\"list-group-item list-group-item-action\">{e.title}</a>";
                });
            }
            else aLinks = "<div class=\"p-4 text-center bg-body-tertiary rounded-3\">No quizzes available.</div>";
            html = html.Replace("<!--quizzes-->", aLinks);

            var response = new HttpResponseMessage();
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private string generateHtml(QuizView quiz)
        {
            var html = "";
            Array.ForEach(quiz.questions, (question) => {
                html += $"<p>{question.title}</p>";
                Array.ForEach(question.answers, (answer) => {
                    html += $"<input type=\"checkbox\" name=\"q{question.id}_a{answer.line}\"><label for=\"q{question.id}_a{answer.line}\">{answer.title}</label>";
                });
            });
            return html;
        }
    }
}
