using MIC.Models.Entities; // 确保引用了 User 实体所在的命名空间
using System.Security.Cryptography;
using System.Text;

namespace MIC.Services
{
    /// <summary>
    /// 认证服务。负责用户登录验证和身份管理。使用 MD5 哈希存储密码（生产环境应使用更安全的算法）
    /// </summary>
    public class AuthService
    {
        /// <summary>
        /// 当前登录的用户信息
        /// </summary>
        public User CurrentUser { get; private set; }

        /// <summary>
        /// 验证用户登录。检查用户名和密码是否正确
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录成功返回 true</returns>
        public bool Login(string username, string password)
        {
            return ValidateLogin(username, password);
        }

        /// <summary>
        /// 执行登录验证逻辑。比较输入的密码哈希与存储的密码哈希
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>验证成功返回 true，并设置 CurrentUser</returns>
        private bool ValidateLogin(string username, string password)
        {
            // 简单演示逻辑（实际应查询数据库）
            string storedHash = GetMd5Hash("123456");
            string inputHash = GetMd5Hash(password);

            if (username == "admin" && inputHash == storedHash)
            {
                // 验证成功，设置当前用户
                CurrentUser = new User
                {
                    Username = username,
                    Role = "Admin"
                };
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算字符串的 MD5 哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>MD5 哈希值（十六进制字符串）</returns>
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