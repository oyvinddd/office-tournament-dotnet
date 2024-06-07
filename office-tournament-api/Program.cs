
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.Middleware;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
using office_tournament_api.Validators;
using System.Reflection;
using System.Text;

namespace office_tournament_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITournamentService, TournamentService>();
            builder.Services.AddScoped<IAccountValidator, AccountValidator>();
            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<PasswordHandler>();
            builder.Services.AddScoped<DTOMapper>();
            builder.Services.AddScoped<EloRating>();
            builder.Services.AddScoped<JwtTokenHandler>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            var skipAuthorization = builder.Configuration.GetSection("SkipAuthorization").Value;

            builder.Services.ConfigureOptions<JwtOptionsSetup>();
            builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt-Auth:Issuer"],
                    ValidAudience = builder.Configuration["Jwt-Auth:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt-Auth:SecretKey"]))
                };
            });

            // Configure the default authorization policy
            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();

            });

            //if(skipAuthorization.ToLower() != "true")
            //{
            //    // JWT Configuration
            //    builder.Services.AddAuthentication(options =>
            //    {
            //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(options =>
            //    {
            //        options.RequireHttpsMetadata = false;
            //        options.SaveToken = true;
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = builder.Configuration.GetSection("Jwt-Auth:Issuer").Value,
            //            ValidAudience = builder.Configuration.GetSection("Jwt-Auth:Audience").Value,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt-Auth:Secret").Value)),
            //            ClockSkew = TimeSpan.Zero
            //        };
            //    });
            //}

            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.SwaggerDoc("v1", new OpenApiInfo
                { Title = "OfficeTournamentApi", Version = "v1" });

                if (skipAuthorization == null || skipAuthorization.ToLower() != "true")
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                   {
                       new OpenApiSecurityScheme
                           {
                               Reference = new OpenApiReference
                                   {
                                       Type=ReferenceType.SecurityScheme,
                                       Id="Bearer"
                                   }
                           },
                           new string[]{}
                   }
               });
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyHeader()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("*"));

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
