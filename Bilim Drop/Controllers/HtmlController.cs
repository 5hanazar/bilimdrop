using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bilim_Drop.Controllers
{
    public class HtmlController : ApiController
    {
        public HttpResponseMessage Get()
        {
            string file = Request.RequestUri.Segments[Request.RequestUri.Segments.Length - 1];
            byte[] fileData = LoadFilesBytes(file);
            if (fileData == null) return new HttpResponseMessage(HttpStatusCode.NotFound);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(fileData);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            return response;
        }
        private byte[] LoadFilesBytes(string file)
        {
            string filePath = $"html/{file}";
            if (File.Exists(filePath)) return File.ReadAllBytes(filePath);
            return null;
        }
    }
}
