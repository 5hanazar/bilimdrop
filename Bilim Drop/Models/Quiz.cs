namespace Bilim_Drop.Models
{
    public class Quiz
    {
        public int id { get; }
        public bool active { get; }
        public string title { get; }
        public string description { get; }
        public string createdDate { get; }

        public Quiz(int id, bool active, string title, string description, string createdDate)
        {
            this.id = id;
            this.active = active;
            this.title = title;
            this.description = description;
            this.createdDate = createdDate;
        }
    }
}
