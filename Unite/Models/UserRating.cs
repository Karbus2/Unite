using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Unite.Models
{
    public class UserRating : Rating
    {
        [DisplayName("ID Użytkownika")]
        public Guid UserId { get; set; }
        [DisplayName("Użytkownik")]
        public virtual ApplicationUser? User { get; set; }
    }
}
