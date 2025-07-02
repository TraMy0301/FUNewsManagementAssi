using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace A01_FuNewsManagament_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FuNewsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


            // Add services to the container.
            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // add odata
            IEdmModel GetEdmModel()
            {
                var builder = new ODataConventionModelBuilder();
                builder.EntitySet<Category>("Categories");
                builder.EntitySet<User>("Users");
                builder.EntitySet<Article>("Articles");
                builder.EntitySet<Tag>("Tags");
                return builder.GetEdmModel();
            }

            builder.Services.AddControllers()
            .AddOData(opt =>
                opt.AddRouteComponents("odata", GetEdmModel())
                    .Select()
                    .Filter()
                    .Expand()
                    .OrderBy()
                    .Count()
                    .SetMaxTop(100))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });


            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Your API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.  
                        Enter 'Bearer' [space] and then your token in the text input below.  
                        Example: 'Bearer abcdef12345'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Authorization",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });


            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ITagService, TagService>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            

            //Auth
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // default là Cookies
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // dùng Google khi cần Challenge
            }).AddCookie()



            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

            })

            //builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
                options.AddPolicy("LecturerOnly", policy => policy.RequireRole("Lecturer"));
            });

            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            //map fe vs be
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("https://localhost:7054") // URL của Frontend
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Nếu cần authentication
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.UseRouting(); // **Phải có**

            app.UseAuthentication(); // nếu có JWT
            app.UseAuthorization();

            app.UseDeveloperExceptionPage();

            app.MapControllers();
            app.MapRazorPages();
            app.MapHub<NotificationHub>("/syncHub");

            app.Run();

        }
    }
}
