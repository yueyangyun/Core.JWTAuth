using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.JWTAuth.Consoles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ////引用System.IdentityModel.Tokens.Jwt
            DateTime utcNow = DateTime.UtcNow;
            string key = "YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv";
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var claims = new List<Claim>() {
                new Claim("ID","1"),
                new Claim("Name","Dick")
            };
            JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: "Dick",
                audience: "audi~~!",
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddMinutes(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

            //生成token
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            //校验token
            var validateParameter = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Dick",
                ValidAudience = "audi~~!",
                IssuerSigningKey = securityKey,
            };
            var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, validateParameter, out SecurityToken validated);

            //解析token
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }

        /// <summary>
        /// 验证身份 验证签名的有效性
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <param name="validatePayLoad">自定义各类验证； 是否包含那种申明，或者申明的值， </param>
        public static bool ValiToken(string encodeJwt, Func<Dictionary<string, string>, bool> validatePayLoad = null)
        {
            string key = "YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv";
            var success = true;
            var jwtArr = encodeJwt.Split('.');
            if (jwtArr.Length < 3)//数据格式都不对直接pass
            {
                return false;
            }
            var header = JsonConvert.DeserializeObject<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[0]));
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[1]));
            //配置文件中取出来的签名秘钥
            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            //验证签名是否正确（把用户传递的签名部分取出来和服务器生成的签名匹配即可）
            success =  string.Equals(jwtArr[2], Base64UrlEncoder.Encode(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1])))));
            if (!success)
            {
                return success;//签名不正确直接返回
            }

            //其次验证是否在有效期内（也应该必须）
            var now = ToUnixEpochDate(DateTime.UtcNow);
            success =  (now >= long.Parse(payLoad["nbf"].ToString()) && now < long.Parse(payLoad["exp"].ToString()));
            if (!success)
            {
                return success;//签名是否过期
            }

            //不需要自定义验证不传或者传递null即可
            if (validatePayLoad == null)
                return true;

            //再其次 进行自定义的验证
            success =  validatePayLoad(payLoad);

            return success;
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }
    }
}
