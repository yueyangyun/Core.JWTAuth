using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.JWTAuth.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        /// <summary>
        /// 必须进行授权才能访问
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "这个接口需要授权才能访问",
                                  "获取用户Name:"+HttpContext.User.Identity.Name,
                                  "获取用户uid:"+HttpContext.User.FindFirst("uid")?.Value,
                                  "获取用户uName:"+HttpContext.User.FindFirst("uName")?.Value
                                };
        }

        /// <summary>
        /// 不需要授权也能访问  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/values/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<string> Get(int id)
        {
            return $"[id={id}]:这个接口不需要授权访问";
        }

    }
}
