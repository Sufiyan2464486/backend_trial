namespace backend_trial.Models.Domain
{
    public class Notification
    {
        public Guid NotificationId { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public NotificationType Type { get; set; }
        public string Message { get; set; } = null!;
        public NotificationStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public User User { get; set; } = null!;
    }
}
