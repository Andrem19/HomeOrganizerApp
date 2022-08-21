using System.Collections.Generic;

namespace HomeOrganizerApp.Models.DTOs
{
    public class AdDto
    {
        public string AuthorName { get; set; }
        public string TextBody { get; set; }
        public bool IsVoting { get; set; }
        public VotingDto Voting { get; set; }
        public int GroupId { get; set; }
    }
}
