using LeaveRequestAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace LeaveRequestAPI.Infrastructure.Persistence;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {    
    }

    public DbSet<LeaveRequest> LeaveRequest => Set<LeaveRequest>();
    public DbSet<Employee> Employee => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LeaveRequest>().Property(lr => lr.Status).HasConversion<string>();

        modelBuilder.Entity<Employee>().Property(e => e.Role).HasConversion<string>();


    }
}
