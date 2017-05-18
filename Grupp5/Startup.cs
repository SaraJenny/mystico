using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Grupp5.Models.Entities;
using Microsoft.Extensions.Configuration;


namespace Grupp5
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
        
            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var conString = Configuration["MysticoConnection"];
            services.AddDbContext<MysticoContext>(o => o.UseSqlServer(conString));
            services.AddDbContext<IdentityDbContext>(o => o.UseSqlServer(conString));
            services.AddSingleton<IConfiguration>(Configuration);
            

            services.AddIdentity<IdentityUser, IdentityRole>(o =>
			{
				o.Password.RequiredLength = 5;
				o.Password.RequireLowercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireDigit = false;
				o.User.RequireUniqueEmail = true;
				o.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
				o.Cookies.ApplicationCookie.LogoutPath = "/Account/SignOut";

			})
			.AddEntityFrameworkStores<IdentityDbContext>()
			.AddDefaultTokenProviders();

            
			services.AddMvc();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
			app.UseStaticFiles();
			app.UseDeveloperExceptionPage();
			app.UseIdentity();

            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = Configuration["GoogleClientId"],
                ClientSecret = Configuration["GoogleClientSecret"],
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}
