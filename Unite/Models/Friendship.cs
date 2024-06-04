using System.ComponentModel;

namespace Unite.Models
{
    public class Friendship : IDbOperationTS
    {
        public enum FriendshipState
        {
            ToAccept,
            Accepted
        }
        public Guid LeftSideId { get; set; }
        [DisplayName("Zapraszający")]
        public virtual ApplicationUser? LeftSide { get; set; }
        public Guid RightSideId { get; set; }
        [DisplayName("Akceptujący")]
        public virtual ApplicationUser? RightSide { get; set; }
        public FriendshipState State { get; set; } = FriendshipState.ToAccept;
        [DisplayName("Zaproszono do znajomych")]
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } 
    }
}
