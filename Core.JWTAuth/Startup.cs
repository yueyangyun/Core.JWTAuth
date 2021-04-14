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
            //���jwt��֤
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        // ��֤token�Ƿ���Ч��token�Ƿ����
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            /* 
                           * Claims (Payload)
                              Claims ���ְ�����һЩ����� token �йص���Ҫ��Ϣ�� JWT ��׼�涨��һЩ�ֶΣ������ѡһЩ�ֶ�:

                              iss: The issuer of the token��token �Ǹ�˭��
                              sub: The subject of the token��token ����
                              exp: Expiration Time�� token ����ʱ�䣬Unix ʱ�����ʽ
                              iat: Issued At�� token ����ʱ�䣬 Unix ʱ�����ʽ
                              jti: JWT ID����Ե�ǰ token ��Ψһ��ʶ
                              ���˹涨���ֶ��⣬���԰��������κ� JSON ���ݵ��ֶΡ�
                           * */

                            ValidateIssuer = true,//�Ƿ���֤Issuer
                            ValidateAudience = true,//�Ƿ���֤Audience
                            ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                            ClockSkew = TimeSpan.Zero, //Ĭ������ķ�����ʱ��ƫ����������Ϊ0��ClockSkew = TimeSpan.Zero
                            ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                            ValidAudience = Configuration["JwtSettings:Issuer"],//Audience
                            ValidIssuer = Configuration["JwtSettings:Audience"],//Issuer���������ǰ��ǩ��jwt������һ��
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
                                    Message = "�Բ��𣡽ӿ������Ȩʧ�ܣ�����Ȩ���ʣ�"
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
            app.UseAuthentication();  //������Ȩ����֤ĳЩ�����Ƿ��з���Ȩ��
            app.UseAuthorization();  //������Ȩ,ע���ȼ�Ȩ����Ȩ
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
