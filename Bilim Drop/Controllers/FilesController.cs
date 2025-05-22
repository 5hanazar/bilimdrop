using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bilim_Drop.Controllers
{
    public class FilesController : ApiController
    {
        public HttpResponseMessage Get()
        {
            string file = Request.RequestUri.Segments[Request.RequestUri.Segments.Length - 1];
            byte[] fileData = LoadFilesBytes(file);
            if (fileData == null) return new HttpResponseMessage(HttpStatusCode.NotFound);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(fileData);
            return response;
        }
        public async Task<IHttpActionResult> Post()
        {
            var fileNameHeader = Request.Headers.GetValues("X-Filename").FirstOrDefault();
            string destinationFolder = "received";
            Directory.CreateDirectory(destinationFolder);
            var filePath = Path.Combine(destinationFolder, fileNameHeader);
            using (var fileStream = File.Create(filePath))
            {
                await Request.Content.CopyToAsync(fileStream);
            }
            return Ok();
        }
        private byte[] LoadFilesBytes(string file)
        {
            string filePath = $"files/{file}";
            if (File.Exists(filePath)) return File.ReadAllBytes(filePath);
            return null;
        }
    }
}
