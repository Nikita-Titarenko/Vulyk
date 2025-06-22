using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vulyk.Models
{
    [Index(nameof(Login), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public string Login { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(20)")]
        public string Password { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(20)")]
        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(20)")]
        public string Name { get; set; } = string.Empty;

        public DateTime LastOnline { get; set; } = DateTime.Now;

        public IEnumerable<UserChat> UserChats { get; set; } = new List<UserChat>();
    }
}
