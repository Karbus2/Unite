using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Unite.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public virtual List<UserEvent>? Events { get; set; }
        public virtual List<Friendship>? LeftSideFriendships { get; set; }
        public virtual List<Friendship>? RightSideFriendships { get; set; }
    }
}
