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
        [IsLower<DateTime>(nameof(End)), FutureDate]
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
        [DisplayName("Opis")]
        public string? Description { get; set; }
        [DisplayName("Maks. liczba uczestników")]
        [Range(2, int.MaxValue)]
        public int? Size { get; set; }
        [DisplayName("Data utworzenia")]
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } 
    }
}
