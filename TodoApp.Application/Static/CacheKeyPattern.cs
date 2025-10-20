using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Static
{
    public class CacheKeyPattern
    {
        public static string All => "users:all";
        public static string ById(string userId) => $"users:id:{userId}";
        public static string ByEmail(string email) => $"users:email:{email}";
        public static string Count => "users:count";
    }
}
