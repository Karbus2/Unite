using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Security.Claims;

namespace Unite.Models
{
    public class ApplicationUser : IdentityUser<Guid>, IDbOperationTS
    {
        public virtual List<UserRating>? UserRatings { get; set; }
        public virtual List<UserRating>? UserReviews { get; set; }
        public virtual List<EventRating>? EventRatings { get; set; }
        public virtual List<EventRating>? EventReviews { get; set; }
        public virtual List<UserEvent>? Events { get; set; }
        public virtual List<Friendship>? LeftSideFriendships { get; set; }
        public virtual List<Friendship>? RightSideFriendships { get; set; }
        [DisplayName("Zarejestrowany")]
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class ApplicationUserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public virtual List<UserRating>? UserRatings { get; set; }
        public virtual List<EventRating>? EventRatings { get; set; }
        public virtual List<UserEvent>? Events { get; set; }
        public virtual List<Friendship>? LeftSideFriendships { get; set; }
        [DisplayName("Średnia ocen użytkownika")]
        public double? AverageScore { get; set; }
        [DisplayName("Średnia ocena organizowanych wydarzeń")]
        public double? AverageEventScore { get; set; }
        [DisplayName("Ilość znajomych")]
        public int FriendshipsCount { get; set; }
        public bool HasCommonEvent { get; set; }
        [DisplayName("Zarejestrowany")]
        public DateTime CreatedDate { get; set; }
        public ApplicationUserDTO(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName!;
            UserRatings = user.UserRatings;
            EventRatings = user.EventRatings;
            Events = user.Events;
            LeftSideFriendships = user.LeftSideFriendships;
            HasCommonEvent = false;
            if(UserRatings != null && UserRatings.Count > 0)
            {
                AverageScore = 0;
                foreach(UserRating rating in UserRatings)
                {
                    AverageScore += rating.Value;
                }
                AverageScore /= UserRatings.Count;
            }
            else
            {
                AverageScore = null;
            }
            if(EventRatings != null && EventRatings.Count > 0)
            {
                AverageEventScore = 0;
                foreach(EventRating rating in EventRatings)
                {
                    AverageEventScore += rating.Value;
                }
                AverageEventScore /= EventRatings.Count;
            }
            else
            {
                AverageEventScore = null;
            }
            if(LeftSideFriendships != null)
            {
                FriendshipsCount = 0;
                foreach(Friendship friendship in LeftSideFriendships)
                {
                    if(friendship.State == Friendship.FriendshipState.Accepted)
                    {
                        FriendshipsCount++;
                    }
                }
            }
        }
        public ApplicationUserDTO(ApplicationUser user, bool hasCommonEvent) : this(user)
        {
            HasCommonEvent = hasCommonEvent;
        }
    }
}
