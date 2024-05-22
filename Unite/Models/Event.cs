﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Unite.Models.CustomAttributes;

namespace Unite.Models
{
    public class Event
    {
        public enum EventScope
        {
            FriendsOnly,
            Private,
            Public
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public EventScope Scope { get; set; } = EventScope.FriendsOnly;
        [IsLower<DateTime>(nameof(End))]
        public DateTime Start { get; set; }
        [IsHigher<DateTime>(nameof(Start))]
        public DateTime End { get; set; }
        public Guid AdminId { get; set; }
        public virtual ApplicationUser? Admin { get; set; }
        public virtual List<UserEvent>? Participants { get; set; }
        public string? Description { get; set; }
        [Range(2, int.MaxValue)]
        public int? Size { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}