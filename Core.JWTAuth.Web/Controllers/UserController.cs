using Core.JWTAuth.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.JWTAuth.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly TokenProviderService _tokenProviderService;
        public UserController(TokenProviderService tokenProviderService)
        {
            _tokenProviderService = tokenProviderService;
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <remarks>
        /// <para/>Author   :  Dick
        /// <para/>Date     :  2021-04-13
        /// </remarks>
        /// <returns></returns>
        [Authorize]
        public IActionResult Users()
        {
            var users = _tokenProviderService.UserList;
            return View(users);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        /// <remarks>
        /// <para/>Author   :  Dick
        /// <para/>Date     :  2021-04-13
        /// </remarks>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult UserInfo(string userId)
        {
            var user = _tokenProviderService.UserList.SingleOrDefault(x => x.USERID == userId);
            return View(user);
        }

        /// <summary>
        /// 非授权页面
        /// </summary>
        /// <remarks>
        /// <para/>Author   :  Dick
        /// <para/>Date     :  2021-04-13
        /// </remarks>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Other()
        {
            return View();
        }
    }
}
