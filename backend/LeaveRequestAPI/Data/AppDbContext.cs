using LeaveRequestAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace LeaveRequestAPI.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {    
    }

    public DbSet<LeaveRequest> LeaveRequest { get; set; } = null!;
    public DbSet<Employee> Employee { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LeaveRequest>().Property(lr => lr.Status).HasConversion<string>();

        modelBuilder.Entity<Employee>().Property(e => e.Role).HasConversion<string>();


    }
}
