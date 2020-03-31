using ANBIDENTITYSERVICE.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ANBIDENTITYSERVICE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public bool Context { get; private set; }

        //public bool context { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrign",
                  builder =>
                  {
                      builder.WithOrigins("https://localhost:44325").AllowAnyHeader().AllowAnyMethod();

                  });
                  
            });
            

            services.AddDbContext<IdentityServiceContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("AppData"));
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = Context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<IdentityServiceContext>()
              .AddDefaultTokenProviders();

            //services.AddIdentity<AppUser, AppRole>()
            //   .AddEntityFrameworkStores<IdentityServiceContext>()
            //   .AddDefaultTokenProviders();

            //services.AddIdentity<AppUser, AppRole>(options =>
            //{
            //    options.Tokens.ProviderMap.Add("Default", new TokenProviderDescriptor(typeof(IUserTwoFactorTokenProvider<AppUser>)));
            //})
            //.AddDefaultTokenProviders();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseCors("AllowMyOrign");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
