using AdvertApi.HealthChecks;
using AdvertApi.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;

namespace AdvertApi
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
            services.AddAutoMapper();
            services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorage>();
            services.AddTransient<StorageHealthCheck>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddHealthChecks().AddCheck<StorageHealthCheck>("Storage");

            services.AddCors(options =>
            {
                options.AddPolicy("AllOrigin", policy=> policy.WithOrigins("*").AllowAnyHeader());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Web Advertisement Apis",
                    Version = "version 1",
                    Contact = new Contact
                    {
                        Name = "Aref Karimi",
                        Email = "a@b.com"
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHealthChecks("/health");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Advert Api");
            });
            app.UseMvc();
        }
    }
}