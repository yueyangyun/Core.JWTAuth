using Core.JWTAuth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.JWTAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加jwt验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        // 验证token是否有效，token是否过期
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            /* 
                           * Claims (Payload)
                              Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:

                              iss: The issuer of the token，token 是给谁的
                              sub: The subject of the token，token 主题
                              exp: Expiration Time。 token 过期时间，Unix 时间戳格式
                              iat: Issued At。 token 创建时间， Unix 时间戳格式
                              jti: JWT ID。针对当前 token 的唯一标识
                              除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
                           * */

                            ValidateIssuer = true,//是否验证Issuer
                            ValidateAudience = true,//是否验证Audience
                            ValidateLifetime = true,//是否验证失效时间
                            ClockSkew = TimeSpan.Zero, //默认允许的服务器时间偏移量，设置为0。ClockSkew = TimeSpan.Zero
                            ValidateIssuerSigningKey = true,//是否验证SecurityKey
                            ValidAudience = Configuration["JwtSettings:Issuer"],//Audience
                            ValidIssuer = Configuration["JwtSettings:Audience"],//Issuer，这两项和前面签发jwt的设置一致
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"]))//SecretKey
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                                logger.LogError("Authentication failed.", context.Exception);
                                ResponseResult responseResult = new ResponseResult<TokenResponseResult>()
                                {
                                    Code = HttpStatusCode.Unauthorized,
                                    Message = "对不起！接口身份授权失败，您无权访问！"
                                };
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.WriteAsync(JsonConvert.SerializeObject(responseResult, Formatting.Indented));
                                return Task.CompletedTask;
                            },
                        };
                    });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core.JWTAuth", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core.JWTAuth v1"));
            }
            app.UseRouting();
            app.UseAuthentication();  //开启鉴权，验证某些方法是否有访问权限
            app.UseAuthorization();  //开启授权,注意先鉴权再授权
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
