namespace Bilim_Drop.Models
{
    public class Quiz
    {
        public int id { get; }
        public bool active { get; }
        public string title { get; }
        public string description { get; }
        public string createdDate { get; }
        public Question[] questions { get; }

        public Quiz(int id, bool active, string title, string description, string createdDate)
        {
            this.id = id;
            this.active = active;
            this.title = title;
            this.description = description;
            this.createdDate = createdDate;
        }

        public Quiz(int id, bool active, string title, string description, string createdDate, Question[] questions) : this(id, active, title, description, createdDate)
        {
            this.questions = questions;
        }
    }

    public class PostQuiz
    {
        public int id { get; }
        public bool active { get; }
        public string title { get; }
        public string description { get; }
        public PostQuestion[] questions { get; }

        public PostQuiz(int id, bool active, string title, string description, PostQuestion[] questions)
        {
            this.id = id;
            this.active = active;
            this.title = title;
            this.description = description;
            this.questions = questions;
        }
    }
}
