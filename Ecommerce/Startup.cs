using Ecommerce.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce
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
            services.AddCors(options =>
            {
                options.AddPolicy("AnyCorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddControllers();

            #region jwt
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #endregion

            #region EF6
            services.AddDbContext<MyContext>(option => option.UseLazyLoadingProxies()
            .UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            #endregion


            services.AddScoped<ArticleService>();
            services.AddScoped<AttributeService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<EmailConfigurationService>();
            services.AddScoped<EmailRegistrationService>();
            services.AddScoped<EmailTemplateService>();
            services.AddScoped<GalleryService>();
            services.AddScoped<MenuService>();
            services.AddScoped<OrderService>();
            services.AddScoped<ProductService>();
            services.AddScoped<ReportService>();
            services.AddScoped<ReviewService>();
            services.AddScoped<UserService>();
            services.AddScoped<WebsiteService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AnyCorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
