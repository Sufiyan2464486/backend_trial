namespace backend_trial.Models.DTO
{
    public class ChangeIdeaStatusRequestDto
    {
        public string Status { get; set; } = null!; // "Draft", "UnderReview", or "Approved"
    }
}
