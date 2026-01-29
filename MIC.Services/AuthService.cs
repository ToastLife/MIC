using MIC.Models.Entities; // 确保引用了 User 实体所在的命名空间
using System.Security.Cryptography;
using System.Text;

namespace MIC.Services
{
    public class AuthService
    {
        // 定义 CurrentUser 属性
        public User CurrentUser { get; private set; }

        public bool Login(string username, string password)
        {
            return ValidateLogin(username, password);
        }

        private bool ValidateLogin(string username, string password)
        {
            // 简单演示逻辑（实际应查询数据库）
            string storedHash = GetMd5Hash("123456");
            string inputHash = GetMd5Hash(password);

            if (username == "admin" && inputHash == storedHash)
            {
                // 变量名 user -> username，并赋值给属性
                CurrentUser = new User
                {
                    Username = username,
                    Role = "Admin"
                };
                return true;
            }
            return false;
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                return sBuilder.ToString();
            }
        }
    }
}