namespace Unite.Models
{
    public class UserEvent : IDbOperationTS
    {
        public enum UserEventRole
        {
            Participant,
            Moderator,
            Admin
        }
        public enum UserEventState
        {
            Accepted,
            Invited
        }
        public Guid ParticipantId { get; set; }
        public virtual ApplicationUser? Participant { get; set; }
        public Guid EventId { get; set; }
        public virtual Event? Event { get; set; }
        public UserEventRole Role { get; set; } = UserEventRole.Participant;
        public UserEventState State { get; set; } = UserEventState.Accepted;
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; }
        public UserEvent() { }
        public UserEvent(Guid participantId, Guid eventId) : this()
        {
            ParticipantId = participantId;
            EventId = eventId;
        }
        public UserEvent(Guid participantId, Guid eventId, UserEventRole role) : this(participantId, eventId)
        {
            Role = role;
        }
    }
}
