using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Unite.Models.CustomAttributes;

namespace Unite.Models
{
    public class Event : IDbOperationTS
    {
        public enum EventScope
        {
            FriendsOnly,
            Private,
            Public
        }

        public Guid Id { get; set; }
        [DisplayName("Tytuł")]
        public string Title { get; set; }
        [DisplayName("Lokalizacja")]
        public string Location { get; set; }
        [DisplayName("Zasięg")]
        public EventScope Scope { get; set; } = EventScope.FriendsOnly;
        [IsLower<DateTime>(nameof(End))]
        [DisplayName("Data rozpoczęcia")]
        public DateTime Start { get; set; }
        [DisplayName("Data zakończenia")]
        [IsHigher<DateTime>(nameof(Start)), FutureDate]
        public DateTime End { get; set; }
        [DisplayName("ID Admina")]
        public Guid AdminId { get; set; }
        [DisplayName("Admin")]
        public virtual ApplicationUser? Admin { get; set; }
        public virtual List<UserEvent>? Participants { get; set; }
        public virtual List<EventRating>? Ratings { get; set; }
        [DisplayName("Opis")]
        public string? Description { get; set; }
        [DisplayName("Maks. liczba uczestników")]
        [Range(2, int.MaxValue)]
        public int? Size { get; set; }
        [DisplayName("Data utworzenia")]
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } 
    }
    public class EventDTO : Event
    {
        public int NumOfParticipants { get; set; }
        public double? AverageScore { get; set; }
        public EventDTO(Event @event)
        {
            Id = @event.Id;
            Title = @event.Title;
            Location = @event.Location;
            Scope = @event.Scope;
            Start = @event.Start;
            End = @event.End;
            AdminId = @event.AdminId;
            Admin = @event.Admin;
            Participants = @event.Participants;
            Ratings = @event.Ratings;
            Description = @event.Description;
            Size = @event.Size;
            CreatedDate = @event.CreatedDate;
            UpdatedDate = @event.UpdatedDate;
            if (Participants == null)
            {
                NumOfParticipants = 0;
            }
            else
            {
                NumOfParticipants = Participants.Where(p => p.State == UserEvent.UserEventState.Accepted).Count();
            }
            if (Ratings != null && Ratings.Count > 0)
            {
                AverageScore = 0;
                foreach(EventRating rating in Ratings)
                {
                    AverageScore += rating.Value;
                }
                AverageScore /= Ratings.Count;
            }
            else
            {
                AverageScore = null;
            }
        }
    }
}
