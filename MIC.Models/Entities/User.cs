using System;

namespace MIC.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // 实际项目中不要存明文
        public string Role { get; set; } // "Admin", "Operator"
        public DateTime LastLoginTime { get; set; }
    }
}