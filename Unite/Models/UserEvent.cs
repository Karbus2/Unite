﻿using System.ComponentModel;

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
        [DisplayName("Uczestnik")]
        public virtual ApplicationUser? Participant { get; set; }
        public Guid EventId { get; set; }
        [DisplayName("Wydarzenie")]
        public virtual Event? Event { get; set; }
        [DisplayName("Rola")]
        public UserEventRole Role { get; set; } = UserEventRole.Participant;
        [DisplayName("Status uczestnictwa")]
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
        public UserEvent(Guid participantId, Guid eventId, UserEventRole role, UserEventState state) : this(participantId, eventId, role)
        {
            State = state;
        }
    }
}
