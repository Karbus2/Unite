namespace Unite.Models
{
    public class UserEvent
    {
        public enum UserEventRole
        {
            Participant,
            Moderator,
            Admin
        }
        public Guid ParticipantId { get; set; }
        public virtual ApplicationUser? Participant { get; set; }
        public Guid EventId { get; set; }
        public virtual Event? Event { get; set; }
        public UserEventRole Role { get; set; } = UserEventRole.Participant;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
