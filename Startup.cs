using AutoMapper;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing
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

            //Configure the DB
            services.AddDbContext<DatabaseContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );



            //add memorycache for throttling and ratelimit
            services.AddMemoryCache();
            services.ConfigureRateLimiting();
            services.AddHttpContextAccessor();

            /* Enable cache*/
            services.ConfigureHttpCacheHeaders();

            /* Configure my identity service*/
            services.AddAuthentication();
            services.ConfigureIdentity();
            /* Configure the JWT*/
            services.ConfigureJWT(Configuration);

            
            /* The original out of the box code is only services.addcontroller, but because I have some circular dependencies (a country has a list of hotels
             * and each hotel has a country), I added newtossoft json serializer and set the option to ignore the loop
             
             Also, I added the caxhe for every control so I dont need to put the [Resoponsecache(duration = 60) in each one]
             */
            services.AddControllers(config => {
                config.CacheProfiles.Add("12SecondsDuration", new CacheProfile
                {
                    Duration = 120
                });
            })
                .AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // Enable CORS, the cors need an object that is the policy rules
            services.AddCors( o => {
                o.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
            /* Configure the AuthManager service*/
            services.AddScoped<IAuthManager, AuthManager>();

            /* Configure the Automapper*/
            services.AddAutoMapper(typeof(MapperInitializer));

            /* Add the UnitOfwork*/
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //configure versioning
            services.ConfigureVersioning();




            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));


            //Cusyom expetionhandler
            app.ConfigureExceptionHandler();

           

            app.UseHttpsRedirection();

            //We tell the app to use the CORS policy we defined in the cofigureServices
            app.UseCors("CorsPolicy");

            //Enable caching middleware
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();

            // Use filtering throttle
            app.UseIpRateLimiting();

            app.UseRouting();

            //Because we are using netcore auth and autorization we need to put it here
            app.UseAuthentication();
            app.UseAuthorization();
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
