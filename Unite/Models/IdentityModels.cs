using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Security.Claims;

namespace Unite.Models
{
    public class ApplicationUser : IdentityUser<Guid>, IDbOperationTS
    {
        public virtual List<UserEvent>? Events { get; set; }
        public virtual List<Friendship>? LeftSideFriendships { get; set; }
        public virtual List<Friendship>? RightSideFriendships { get; set; }
        [DisplayName("Zarejestrowany")]
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
