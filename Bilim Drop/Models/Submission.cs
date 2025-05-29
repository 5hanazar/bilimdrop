namespace Bilim_Drop.Models
{
    public class Submission
    {
        public int id { get; }
        public bool isSubmitted { get; }
        public string username { get; }
        public int quizId { get; }
        public string quizJ { get; }
        public string answersJ { get; }
        public string createdDate { get; }

        public Submission(int id, bool isSubmitted, string username, int quizId, string quizJ, string answersJ, string createdDate)
        {
            this.id = id;
            this.isSubmitted = isSubmitted;
            this.username = username;
            this.quizId = quizId;
            this.quizJ = quizJ;
            this.answersJ = answersJ;
            this.createdDate = createdDate;
        }
    }
}
