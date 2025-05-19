namespace Bilim_Drop.Models
{
    public class Question
    {
        public int id { get; }
        public int questionType { get; }
        public string title { get; }
        public Answer[] answers { get; }

        public Question(int id, int questionType, string title, Answer[] answers)
        {
            this.id = id;
            this.questionType = questionType;
            this.title = title;
            this.answers = answers;
        }
    }

    public class PostQuestion
    {
        public int id { get; }
        public int line { get; }
        public int questionType { get; }
        public string title { get; }
        public PostAnswer[] answers { get; }

        public PostQuestion(int id, int line, int questionType, string title, PostAnswer[] answers)
        {
            this.id = id;
            this.line = line;
            this.questionType = questionType;
            this.title = title;
            this.answers = answers;
        }
    }
}
