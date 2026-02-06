using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Xml.Linq;

namespace backend_trial.Models.Domain
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public UserRole Role { get; set; }
        public string Email { get; set; } = null!;
        public string? Department { get; set; } = null!;
        public UserStatus Status { get; set; }

        public ICollection<Idea> SubmittedIdeas { get; set; } = new List<Idea>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Review> ReviewsAuthored { get; set; } = new List<Review>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
