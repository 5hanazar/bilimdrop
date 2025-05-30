namespace Bilim_Drop.Models
{
    public class QuizView
    {
        public int id { get; }
        public string title { get; }
        public string description { get; }
        public string createdDate { get; }
        public QuestionView[] questions { get; }

        public QuizView(int id, string title, string description, string createdDate, QuestionView[] questions)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.createdDate = createdDate;
            this.questions = questions;
        }
    }

    public class QuestionView
    {
        public int id { get; }
        public int questionType { get; }
        public string title { get; }
        public AnswerView[] answers { get; }

        public QuestionView(int id, int questionType, string title, AnswerView[] answers)
        {
            this.id = id;
            this.questionType = questionType;
            this.title = title;
            this.answers = answers;
        }
    }

    public class AnswerView
    {
        public int line { get; }
        public string title { get; }

        public AnswerView(int line, string title)
        {
            this.line = line;
            this.title = title;
        }
    }
}
