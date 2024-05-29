
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using office_tournament_api.office_tournament_db;
using System.Reflection;

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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            var skipAuthorization = builder.Configuration.GetSection("SkipAuthorization").Value;
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
