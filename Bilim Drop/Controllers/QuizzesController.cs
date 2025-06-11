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
                var cookie = Request.Headers.GetCookies("sub_id").FirstOrDefault();

                int sub_id = 0;
                if (cookie != null)
                {
                    var _buf = int.Parse(cookie["sub_id"].Value);
                    var _el = await repo.getSubmission(_buf);
                    if (_el != null && !_el.isSubmitted) sub_id = _buf;
                }

                if (sub_id == 0)
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
                    sub_id = await repo.insertOrUpdateSubmission(new PostSubmission(0, false, "steve", int.Parse(_id), JsonConvert.SerializeObject(quizView), ""));
                }
                var submission = await repo.getSubmission(sub_id);
                var r = new HttpResponseMessage();
                r.Content = new StringContent(generateHtml(submission, JsonConvert.DeserializeObject<QuizView>(submission.quizJ)));
                var cookieHeader = new CookieHeaderValue("sub_id", sub_id.ToString());
                r.Headers.AddCookies(new CookieHeaderValue[] { cookieHeader });
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
                    aLinks += $"<a href=\"/quizzes?id={e.id}\" class=\"list-group-item list-group-item-action\">{e.title}</a>";
                });
            }
            else aLinks = "<div class=\"p-5 text-center bg-body-tertiary rounded-3\">No quizzes available.</div>";
            html = html.Replace("<!--quizzes-->", aLinks);

            var response = new HttpResponseMessage();
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private string generateHtml(Submission submission, QuizView quiz)
        {
            var html = File.ReadAllText("html/quiz.html");
            var chunk = "";
            Array.ForEach(quiz.questions, (question) => {
                chunk += $"<div class='question_container'><p class='question'>{question.title}</p><div class='options' id='options'>";
                Array.ForEach(question.answers, (answer) => {
                    chunk += $"<div class='option'><input type='checkbox' id='q{question.id}_a{answer.line}' autocomplete='off' /><label for='q{question.id}_a{answer.line}'>{answer.title}</label></div>";
                });
                chunk += "</div></div>";
            });
            html = html.Replace("<!--questions-->", chunk).Replace("<!--title-->", quiz.title).Replace("<!--submissionCreatedDate-->", submission.createdDate).Replace("/*submissionId*/", submission.id.ToString());
            return html;
        }

        public async Task<IHttpActionResult> Post([FromBody] Submitted body)
        {
            var submission = await repo.getSubmission(body.submissionId);
            await repo.insertOrUpdateSubmission(new PostSubmission(submission.id, true, submission.username, submission.quizId, submission.quizJ, JsonConvert.SerializeObject(body.answers)));
            return Ok();
        }
    }
}
