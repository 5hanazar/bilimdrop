using System.Drawing;

namespace Bilim_Drop.Models
{
    public class FileDto
    {
        public Image image { get; }
        public string name { get; }
        public string description { get; }

        public FileDto(Image image, string name, string description)
        {
            this.image = image;
            this.name = name;
            this.description = description;
        }
    }
}
