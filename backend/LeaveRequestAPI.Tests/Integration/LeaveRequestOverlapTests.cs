using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Domain.Enums;
using LeaveRequestAPI.Infrastructure.Persistence;

namespace LeaveRequestAPI.Tests.Integration;

public class LeaveRequestOverlapTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LeaveRequestOverlapTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateLeaveRequest_WhenOverlapExists_ShouldReturn400WithSpecificMessage()
    {
        // Arrange - Setup database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Limpiar y preparar datos
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Crear empleado y manager
        var employee = new Employee { Id = 1, Name = "Juan", Email = "juan@test.com", Role = EmployeeRole.Employee };
        var manager = new Employee { Id = 2, Name = "Manager", Email = "manager@test.com", Role = EmployeeRole.Manager };
        context.Employee.AddRange(employee, manager);

        // Crear solicitud aprobada
        var approvedRequest = new LeaveRequest
        {
            Id = 1,
            EmployeeId = 1,
            StartDate = new DateTime(2026, 2, 15),
            EndDate = new DateTime(2026, 2, 25),
            Status = LeaveStatus.Approved,
            Reason = "Vacaciones aprobadas"
        };
        context.LeaveRequest.Add(approvedRequest);
        await context.SaveChangesAsync();

        // Nueva solicitud con overlap
        var newRequest = new LeaveRequestCreateDTO
        {
            EmployeeId = 1,
            StartDate = new DateTime(2026, 2, 20),
            EndDate = new DateTime(2026, 3, 5),
            Reason = "Nueva solicitud"
        };

        // Act - Enviar request con headers de autenticación
        _client.DefaultRequestHeaders.Add("X-User-Id", "1");
        _client.DefaultRequestHeaders.Add("X-User-Role", "Employee");
        var response = await _client.PostAsJsonAsync("/api/leaverequests", newRequest, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("se solapan con una solicitud ya aprobada", content);
        Assert.Contains("15/02/2026", content);
        Assert.Contains("25/02/2026", content);
        Assert.Contains("OVERLAPPING_REQUEST", content);
    }
}
