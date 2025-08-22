namespace Vulyk.Domain.Models
{
    public class Chat
    {
        public int Id { get; set; }

        public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}
