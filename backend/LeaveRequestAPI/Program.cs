using FluentValidation;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Application.Interfaces;
using LeaveRequestAPI.Application.Services;
using LeaveRequestAPI.Application.Validators;
using LeaveRequestAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace LeaveRequestAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        // FluentValidation
        builder.Services.AddScoped<IValidator<LeaveRequestCreateDTO>, LeaveRequestCreateDTOValidator>();
        builder.Services.AddScoped<IValidator<LeaveRequestUpdateStatusDTO>, LeaveRequestUpdateStatusDTOValidator>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Mapper
        builder.Services.AddAutoMapper(typeof(LeaveRequestMapper));

        builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

        // DB Context
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("dbConnection"));
        });

        var app = builder.Build();

        // Llamar a DbInitializer y registrar logs
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                DbInitializer.Initialize(context);
                logger.LogInformation("Base de datos inicializada correctamente.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al inicializar la base de datos.");
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
