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
        [DisplayName("ID Admina")]
        public Guid AdminId { get; set; }
        [DisplayName("Admin")]
        public virtual ApplicationUser? Admin { get; set; }
    }
}
