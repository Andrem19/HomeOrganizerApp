using System.Collections.Generic;

namespace HomeOrganizerApp.Models.DTOs
{
    public class VotingDto
    {
        public List<VariantDto> Variants { get; set; }
        public bool IsSecret { get; set; }
    }
}
