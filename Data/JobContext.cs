using Microsoft.EntityFrameworkCore;
using JobSeekerApi.Models;

namespace JobSeekerApi.Data;

public class JobContext : DbContext
{
    public JobContext(DbContextOptions<JobContext> options) : base(options){}
    
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Category> Categories { get; set; }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Job>()
    //         .HasOne(j => j.Category)
    //         .WithMany()
    //         .HasForeignKey(j => j.CategoryId)
    //         .OnDelete(DeleteBehavior.Cascade);
    // }
}