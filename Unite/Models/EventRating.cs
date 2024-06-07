using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Unite.Models
{
    public class EventRating : Rating
    {
        [DisplayName("ID Wydarzenia")]
        public Guid EventId { get; set; }
        [DisplayName("Wydarzenie")]
        public virtual Event? Event { get; set; }
        //[DisplayName("ID Recenzenta")]
        //public Guid ReviewerId { get; set; }
        //[DisplayName("Recenzent")]
        //public virtual ApplicationUser? Reviewer { get; set; }
        [DisplayName("ID Admina")]
        public Guid AdminId { get; set; }
        [DisplayName("Admin")]
        public virtual ApplicationUser? Admin { get; set; }
        //[DisplayName("ID Recenzenta")]
        //public Guid ReviewerId { get; set; }
        //[DisplayName("Recenzent")]
        //public virtual ApplicationUser? Reviewer { get; set; }
        //[Range(0, 5)]
        //[DisplayName("Ocena")]
        //public int Rating { get; set; }
        //[Length(0, 512)]
        //[DisplayName("Opinia")]
        //public string Review { get; set; } = string.Empty;
        //[DisplayName("Data oceny")]
        //public DateTime CreatedDate { get; set; }
        //public DateTime UpdatedDate { get; set; }
    }
}
