﻿namespace Vulyk.Models
{
    public class UserChat
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }

        public User User { get; set; } = null!;
        public Chat Chat { get; set; } = null!;

        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
    }
}
