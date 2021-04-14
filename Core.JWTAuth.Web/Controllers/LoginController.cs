using Core.JWTAuth.Web.Models;
using Core.JWTAuth.Web.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.JWTAuth.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult LoginHomePage()
        {
            string userId = HttpContext.Session.GetString("USERID");
            if (!string.IsNullOrEmpty(userId))
            {
                return Redirect("~/User/Users");
            }
            return View();
        }

        public IActionResult LogIn(User user)
        {
            TokenProviderService _tokenProvider = new TokenProviderService();
            //Authenticate user
            var userToken = _tokenProvider.GetToken(user.USERID.Trim(), user.PASSWORD.Trim());
            if (userToken != null)
            {
                //Save token in session object
                HttpContext.Session.SetString("USERID", user.USERID.Trim());
                HttpContext.Session.SetString("JWToken", userToken);
            }
            return Redirect("~/User/Users");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login/LoginHomePage");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}