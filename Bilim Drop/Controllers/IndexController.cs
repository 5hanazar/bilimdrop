using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Bilim_Drop.Controllers
{
    public class IndexController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("<h1 style=\"text-align: center; font-family: Arial\">Welcome to Bilim Drop!</h1>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
