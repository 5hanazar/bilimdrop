namespace Bilim_Drop.Models
{
    public class Question
    {
        public int id { get; }
        public int quizId { get; }
        public int questionType { get; }
        public string title { get; }

        public Question(int id, int quizId, int questionType, string title)
        {
            this.id = id;
            this.quizId = quizId;
            this.questionType = questionType;
            this.title = title;
        }
    }

}
