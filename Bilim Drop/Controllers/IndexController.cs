using Bilim_Drop.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Bilim_Drop.Controllers
{
    public class IndexController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var html = File.ReadAllText("html/index.html");
            var aLinks = "";
            var files = _getFiles();
            if (files.Count > 0)
            {
                files.ForEach(e =>
                {
                    aLinks += $"<a href=\"files/{e.name}\" class=\"list-group-item list-group-item-action\">{e.name}</a>";
                });
            }
            else aLinks = "<div class=\"p-4 text-center bg-body-tertiary rounded-3\">No materials available.</div>";
            html = html.Replace("<!--files-->", aLinks);

            var response = new HttpResponseMessage();
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private List<FileDto> _getFiles() {
            var list = new List<FileDto>();
            try
            {
                var files = Directory.GetFiles($"files");
                foreach (var f in files)
                {
                    list.Add(new FileDto(Icon.ExtractAssociatedIcon(f).ToBitmap(), Path.GetFileName(f), ""));
                }
            }
            catch (Exception e) { }
            return list;
        }
    }
}
