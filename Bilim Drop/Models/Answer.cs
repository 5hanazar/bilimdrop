namespace Bilim_Drop.Models
{
    public class Answer
    {
        public int line { get; }
        public string title { get; }
        public bool isCorrect { get; }

        public Answer(int line, string title, bool isCorrect)
        {
            this.line = line;
            this.title = title;
            this.isCorrect = isCorrect;
        }
    }

    public class PostAnswer
    {
        public int line { get; }
        public string title { get; }
        public bool isCorrect { get; }

        public PostAnswer(int line, string title, bool isCorrect)
        {
            this.line = line;
            this.title = title;
            this.isCorrect = isCorrect;
        }
    }
}
