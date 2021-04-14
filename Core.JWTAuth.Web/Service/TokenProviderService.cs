using Core.JWTAuth.Web.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.JWTAuth.Web.Service
{
    public class TokenProviderService
    {
        //Using hard coded collection list as Data Store for demo purposes
        //In reality, User data comes from Database or other Data Source.
        public List<User> UserList = new List<User>
        {
            new User { USERID = "jsmith@email.com", PASSWORD = "test",
                       EMAILID = "jsmith@email.com", FIRST_NAME = "John",
                       LAST_NAME = "Smith", PHONE = "356-735-2748",
                       ACCESS_LEVEL = "Director", READ_ONLY = "true" },
            new User { USERID = "srob@email.com", PASSWORD = "test",
                       FIRST_NAME = "Steve", LAST_NAME = "Rob",
                       EMAILID = "srob@email.com", PHONE = "567-479-8537",
                       ACCESS_LEVEL = "Supervisor", READ_ONLY = "false" },
            new User { USERID = "dwill@email.com", PASSWORD = "test",
                       FIRST_NAME = "DJ", LAST_NAME = "Will",
                       EMAILID = "dwill@email.com", PHONE = "599-306-6010",
                       ACCESS_LEVEL = "Analyst", READ_ONLY = "false" },
            new User { USERID = "JBlack@email.com", PASSWORD = "test",
                       FIRST_NAME = "Joe", LAST_NAME = "Black",
                       EMAILID = "JBlack@email.com", PHONE = "764-460-8610",
                       ACCESS_LEVEL = "Analyst", READ_ONLY = "true" }
            // 使用“ACCESS_LEVEL”和“READ_ONLY”声明来设置用户权限
        };

        //Using hard coded values in claims collection list as Data Store for demo. 
        //In reality, User data comes from Database or other Data Source.
        public IEnumerable<Claim> GetUserClaims(User user)
        {
            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.FIRST_NAME + " " + user.LAST_NAME), // 页面展示
                new Claim("USERID", user.USERID),
                new Claim("EMAILID", user.EMAILID),
                new Claim("PHONE", user.PHONE),
                new Claim("ACCESS_LEVEL", user.ACCESS_LEVEL.ToUpper()),
                new Claim("READ_ONLY", user.READ_ONLY.ToUpper())
            };
            return claims;
        }

        public string GetToken(string userId, string password)
        {
            TokenProviderService tokenProvider = new TokenProviderService();
            //Get user details for the user who is trying to login
            var user = tokenProvider.UserList.SingleOrDefault(x => x.USERID == userId);

            //Authenticate User, Check if it’s a registered user in Database
            if (user == null)
                return null;

            //If it's registered user, check user password stored in Database 
            //For demo, password is not hashed. Simple string comparison 
            //In real, password would be hashed and stored in DB. Before comparing, hash the password
            if (password == user.PASSWORD)
            {
                //Authentication successful, Issue Token with user credentials
                //Provide the security key which was given in the JWToken configuration in Startup.cs
                var key = Encoding.ASCII.GetBytes
                          ("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                //Generate Token for user 
                var JWToken = new JwtSecurityToken(
                    issuer: "http://localhost:45195/",  //jwt签发者
                    audience: "http://localhost:45195/",  //接收jwt的一方
                    claims: tokenProvider.GetUserClaims(user),
                    expires: DateTime.Now.AddHours(8), //jwt的过期时间，这个过期时间必须要大于签发时间
                                                       //Using HS256 Algorithm to encrypt Token
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha256Signature)
                );
                var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                return token;
            }
            else
            {
                return null;
            }
        }
    }
}
