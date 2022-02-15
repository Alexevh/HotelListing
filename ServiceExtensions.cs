using HotelListing.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace HotelListing
{

    /* Because the 'configure' services at startup is now too long, I will separate some configurations, this is an extention class with extension methods*/
    public static class ServiceExtensions
    {

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            /* WE ARE CONFIGURING THE identity policies, we can enforce password legnth and many things, for now I will only configure the email*/
            var builder = services.AddIdentityCore<Data.ApiUser>( q => q.User.RequireUniqueEmail = true);
            /* We will use the default identity role*/
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }


        public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
        {
            var jwtSettings = Configuration.GetSection("Jwt");
            var key = jwtSettings.GetSection("KEY").Value;
            services.AddAuthentication(op =>
            {
                //when someone authetnicates, check for a jwt is saying
               op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });
        }
    }
}
