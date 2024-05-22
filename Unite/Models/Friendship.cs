namespace Unite.Models
{
    public class Friendship
    {
        public enum FriendshipState
        {
            ToAccept,
            Accepted
        }
        public Guid LeftSideId { get; set; }
        public virtual ApplicationUser? LeftSide { get; set; }
        public Guid RightSideId { get; set; }
        public virtual ApplicationUser? RightSide { get; set; }
        public FriendshipState State { get; set; } = FriendshipState.ToAccept;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
