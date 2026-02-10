namespace backend_trial.Models.Domain
{
    public class Category
    {
        public Guid CategoryId { get; set; } 
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
