namespace backend_trial.Models.DTO
{
    public class ReviewWithIdeaRequestDto
    {
        public Guid IdeaId { get; set; }
        public string Feedback { get; set; } = null!;
        public string Decision { get; set; } = null!; // "Approve" or "Reject"
    }
}
