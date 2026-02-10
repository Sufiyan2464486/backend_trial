namespace backend_trial.Models.Domain
{
    public class Vote
    {
        public Guid VoteId { get; set; } 

        public Guid IdeaId { get; set; }
        public Guid UserId { get; set; }

        public VoteType VoteType { get; set; }

        public Idea Idea { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
