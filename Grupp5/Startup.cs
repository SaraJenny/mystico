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

namespace Grupp5
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
			var conString = @"Server=tcp:mystico.database.windows.net,1433;Initial Catalog=Mystico;Persist Security Info=False;User ID=grupp5;Password=Pillow123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
			services.AddDbContext<IdentityDbContext>(o => o.UseSqlServer(conString));
			services.AddIdentity<IdentityUser, IdentityRole>(o =>
			{
				o.Password.RequiredLength = 5;
				o.Password.RequireLowercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireDigit = false;
				o.User.RequireUniqueEmail = true;
				o.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
			})
			.AddEntityFrameworkStores<IdentityDbContext>()
			.AddDefaultTokenProviders();


			services.AddMvc();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
			app.UseDeveloperExceptionPage();
			app.UseIdentity();
			app.UseMvcWithDefaultRoute();
        }
    }
}
