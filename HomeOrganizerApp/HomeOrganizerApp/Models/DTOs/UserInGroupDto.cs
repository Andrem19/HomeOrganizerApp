namespace HomeOrganizerApp.Models.DTOs
{
    public class UserInGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public ROLE Role { get; set; }
        public MainPageChoise MainPageChoise { get; set; }
    }
    public enum ROLE
    {
        CREATOR,
        MODERATOR,
        MEMBER
    }
    public enum MainPageChoise
    {
        Ad,
        PayLoads
    }
}
