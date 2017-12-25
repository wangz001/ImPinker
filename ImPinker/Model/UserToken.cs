using System;

namespace ImModel
{
    public class UserToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 登录成功的token
        /// </summary>
        public string Token { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
