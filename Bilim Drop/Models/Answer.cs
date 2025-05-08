namespace Bilim_Drop.Models
{
    public class Answer
    {
        public int id { get; }
        public int questionId { get; }
        public string title { get; }
        public bool isCorrect { get; }

        public Answer(int id, int questionId, string title, bool isCorrect)
        {
            this.id = id;
            this.questionId = questionId;
            this.title = title;
            this.isCorrect = isCorrect;
        }
    }
}
