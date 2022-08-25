namespace HomeOrganizerApp.Models.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsDescription { get; set; }
        public string Description { get; set; }
        public TYPE Type { get; set; }
        public bool Complete { get; set; }
        public string NameWhoCompletLast { get; set; }
        public string Color { get; set; }
        public int PayloadId { get; set; }
        public int GroupId { get; set; }
    }
    public enum TYPE
    {
        EVERYDAY,
        ALLWAYS,
        ONCE
    }
}
