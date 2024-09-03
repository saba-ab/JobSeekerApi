using Microsoft.EntityFrameworkCore;
using JobSeekerApi.Models;

namespace JobSeekerApi.Data;

public class JobContext : DbContext
{
    public JobContext(DbContextOptions<JobContext> options) : base(options){}
    
    public DbSet<Job> Jobs { get; set; }
}