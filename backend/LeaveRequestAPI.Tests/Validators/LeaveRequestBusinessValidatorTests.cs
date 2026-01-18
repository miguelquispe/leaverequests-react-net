using Microsoft.EntityFrameworkCore;
using LeaveRequestAPI.Application.Validators;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Application.Common;
using LeaveRequestAPI.Domain.Entities;
using LeaveRequestAPI.Domain.Enums;
using LeaveRequestAPI.Infrastructure.Persistence;

namespace LeaveRequestAPI.Tests.Validators;

public class LeaveRequestBusinessValidatorTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new AppDbContext(options);
    }

    [Fact]
    public async Task ValidateCreateAsync_WhenOverlappingApprovedRequest_ShouldReturnFailure()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var validator = new LeaveRequestBusinessValidator(context);

        // Crear empleado
        var employee = new Employee { Id = 1, Name = "Juan", Email = "juan@test.com", Role = EmployeeRole.Employee };
        context.Employee.Add(employee);

        // Crear solicitud APROBADA existente
        var existingRequest = new LeaveRequest
        {
            Id = 1,
            EmployeeId = 1,
            StartDate = new DateTime(2026, 2, 15),
            EndDate = new DateTime(2026, 2, 25),
            Status = LeaveStatus.Approved,
            Reason = "Vacaciones"
        };
        context.LeaveRequest.Add(existingRequest);
        await context.SaveChangesAsync();

        // Nueva solicitud que se SOLAPA
        var newRequest = new LeaveRequestCreateDTO
        {
            EmployeeId = 1,
            StartDate = new DateTime(2026, 2, 20), // Se solapa con 15-25
            EndDate = new DateTime(2026, 3, 5),
            Reason = "Más vacaciones"
        };

        // Act
        var result = await validator.ValidateCreateAsync(newRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BusinessErrorCodes.OVERLAPPING_REQUEST, result.ErrorCode);
        Assert.Contains("se solapan con una solicitud ya aprobada", result.ErrorMessage);
        Assert.Contains("15/02/2026", result.ErrorMessage);
        Assert.Contains("25/02/2026", result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateCreateAsync_WhenNoOverlapping_ShouldReturnSuccess()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var validator = new LeaveRequestBusinessValidator(context);

        var employee = new Employee { Id = 1, Name = "Juan", Email = "juan@test.com", Role = EmployeeRole.Employee };
        context.Employee.Add(employee);

        // Solicitud aprobada existente
        var existingRequest = new LeaveRequest
        {
            Id = 1,
            EmployeeId = 1,
            StartDate = new DateTime(2026, 2, 15),
            EndDate = new DateTime(2026, 2, 25),
            Status = LeaveStatus.Approved,
            Reason = "Vacaciones"
        };
        context.LeaveRequest.Add(existingRequest);
        await context.SaveChangesAsync();

        // Nueva solicitud SIN SOLAPE
        var newRequest = new LeaveRequestCreateDTO
        {
            EmployeeId = 1,
            StartDate = new DateTime(2026, 3, 1), // Después del 25/02
            EndDate = new DateTime(2026, 3, 10),
            Reason = "Más vacaciones"
        };

        // Act
        var result = await validator.ValidateCreateAsync(newRequest);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("2026-02-10", "2026-02-20")] // Solape al inicio
    [InlineData("2026-02-20", "2026-03-05")] // Solape al final  
    [InlineData("2026-02-10", "2026-03-05")] // Engloba totalmente
    [InlineData("2026-02-16", "2026-02-24")] // Dentro totalmente
    [InlineData("2026-02-15", "2026-02-25")] // Fechas exactas
    public async Task ValidateCreateAsync_VariousOverlapScenarios_ShouldReturnFailure(string startDate, string endDate)
    {
        // Arrange
        using var context = GetInMemoryContext();
        var validator = new LeaveRequestBusinessValidator(context);

        var employee = new Employee { Id = 1, Name = "Juan", Email = "juan@test.com", Role = EmployeeRole.Employee };
        context.Employee.Add(employee);

        // Solicitud aprobada: 15 Feb - 25 Feb
        var existingRequest = new LeaveRequest
        {
            EmployeeId = 1,
            StartDate = new DateTime(2026, 2, 15),
            EndDate = new DateTime(2026, 2, 25),
            Status = LeaveStatus.Approved,
            Reason = "Vacaciones"
        };
        context.LeaveRequest.Add(existingRequest);
        await context.SaveChangesAsync();

        var newRequest = new LeaveRequestCreateDTO
        {
            EmployeeId = 1,
            StartDate = DateTime.Parse(startDate),
            EndDate = DateTime.Parse(endDate),
            Reason = "Test"
        };

        // Act
        var result = await validator.ValidateCreateAsync(newRequest);

        // Assert
        Assert.False(result.IsSuccess, $"Should fail for dates {startDate} - {endDate}");
        Assert.Equal(BusinessErrorCodes.OVERLAPPING_REQUEST, result.ErrorCode);
    }
}
