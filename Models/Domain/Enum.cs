namespace backend_trial.Models.Domain
{
    public enum UserRole { Employee, Manager, Admin }
    public enum UserStatus { Active, Inactive }
    public enum IdeaStatus { Draft, UnderReview, Approved }
    public enum VoteType { Upvote, Downvote }
    public enum ReviewDecision { Approve, Reject }
    public enum NotificationType { NewIdea, ReviewDecision }
    public enum NotificationStatus { Unread, Read }
}

