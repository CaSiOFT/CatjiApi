using CatjiApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

//Scaffold-DbContext "Data Source=myweb1008.xyz:1521/orcl;User Id=Catji;Password=tongji;Persist Security Info=True;" Oracle.EntityFrameworkCore -outputdir Models -f

namespace CatjiApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ModelContext>(opt => opt.UseOracle(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ModelContext>(opt => opt.UseOracle(Configuration.GetConnectionString("DebugConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication("Cookies").AddCookie("Cookies", o =>
            {
                o.SlidingExpiration = true;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                o.EventsType = typeof(CustomCookieAuthenticationEvents);
                o.Cookie.SameSite = SameSiteMode.None;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddCors(option =>
            {
                option.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                        // .WithOrigins("*", "http://catji.site", "http://www.catji.site", "http://cocat.top", "http://kingzoey.github.io");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // using (var srvScope=app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()){
            //     var context=srvScope.ServiceProvider.GetRequiredService<>();
            //     context.Database.EnsureCreated();
            // }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            // app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseFileServer();
            app.UseAuthentication();
            app.UseMvc();
        }
    }

    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ModelContext _context;

        public CustomCookieAuthenticationEvents(ModelContext context)
        {
            _context = context;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            // Look for the LastChanged claim.
            var lastChanged = (from c in userPrincipal.Claims
                               where c.Type == "LastChanged"
                               select c.Value).FirstOrDefault();
            var usid = (from c in userPrincipal.Claims
                        where c.Type == "User"
                        select c.Value).FirstOrDefault();

            var user0 = await _context.Users.FindAsync(Convert.ToInt32(usid));

            try
            {
                if (string.IsNullOrEmpty(lastChanged) || user0 == null ||
                    user0.ChangedTime.ToTimestamp() != Convert.ToInt32(lastChanged))
                {
                    context.RejectPrincipal();

                    await context.HttpContext.SignOutAsync();
                }
            }
            catch
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync();
            }
        }
    }
}