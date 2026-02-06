namespace backend_trial.Models.Domain
{
    public class Comment
    {
        public Guid CommentId { get; set; } = Guid.NewGuid();

        public Guid IdeaId { get; set; }
        public Guid UserId { get; set; }

        public string Text { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public Idea Idea { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
