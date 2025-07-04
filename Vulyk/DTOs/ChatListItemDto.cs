﻿using System.ComponentModel.DataAnnotations;

namespace Vulyk.DTOs
{
    public class ChatListItemDto
    {
        public int ChatId { get; set; }

        public int UserId { get; set; }
        [Required]
        public string Name = string.Empty;

        public DateTime? LastMessageDateTime { get; set; }

        public string? LastMessageText { get; set; } = string.Empty;
    }
}
