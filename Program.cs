using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ToyEcommerceASPNET.Data;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Add services to the container.
            builder.Services.Configure<DatabaseSettings>(
                builder.Configuration.GetSection(nameof(DatabaseSettings)));

            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<IUserService, UserService>();

            builder.Services.AddTransient<ICartService, CartService>();

            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();

            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

            builder.Services.AddMvc();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"])),
    //                ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAuthenticated", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));

                options.AddPolicy("IsMatchedUser", policy => policy.RequireAssertion(context =>
                {
                    var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userIdFromRoute = context.Resource as string;

                    return userIdClaim == userIdFromRoute;
                }));

                options.AddPolicy("IsAdminOrMatchUser", policy => policy.RequireAssertion(context =>
                {
                    var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userIdFromRoute = context.Resource as string;
                    var role = context.User.FindFirstValue(ClaimTypes.Role);

                    return role == "Admin" || userIdClaim == userIdFromRoute;
                }));
            });

            // Create swagger document for APIs
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "EcommerceAPI",
                    Version = "v1"
                });
            });

            // Add cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Dependency Injection of DbContext Class
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("MyCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}