using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.JWTAuth.Web.Models
{
    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string USERID { get; set; }
        /// <summary>
        /// 密码 
        /// </summary>
        public string PASSWORD { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string FIRST_NAME { get; set; }
        /// <summary>
        /// 性
        /// </summary>
        public string LAST_NAME { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string EMAILID { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string PHONE { get; set; }
        /// <summary>
        /// 用户权限级别：Director、Supervisor、Analyst
        /// </summary>
        public string ACCESS_LEVEL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string READ_ONLY { get; set; }
    }
}
