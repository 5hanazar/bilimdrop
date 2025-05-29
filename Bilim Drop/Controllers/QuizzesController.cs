using System;
using System.IO;
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
    }
}
