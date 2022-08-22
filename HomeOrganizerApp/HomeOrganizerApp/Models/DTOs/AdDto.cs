﻿using System.Collections.Generic;

namespace HomeOrganizerApp.Models.DTOs
{
    public class AdDto
    {
        public string AuthorName { get; set; }
        public string TextBody { get; set; }
        public string AuthorId { get; set; }
        public string AuthorAvatar { get; set; }
        public bool IsVoting { get; set; }
        public VotingDto Voting { get; set; }
        public List<string> Acquainted { get; set; } = new List<string>();
        public int GroupId { get; set; }
    }
}
