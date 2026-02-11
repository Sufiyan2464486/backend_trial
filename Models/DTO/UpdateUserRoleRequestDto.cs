namespace backend_trial.Models.DTO
{
    public class UpdateUserRoleRequestDto
    {
        public string Role { get; set; } = null!; // "Employee", "Manager", or "Admin"
    }
}
