using DotNetEnv;
using JobSeekerApi.Data;
using JobSeekerApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var connectionString = $"Server={Environment.GetEnvironmentVariable("DATABASE_SERVER")};" +
                       $"Database={Environment.GetEnvironmentVariable("DATABASE_NAME")};" +
                       $"User Id={Environment.GetEnvironmentVariable("DATABASE_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};" +
                       "TrustServerCertificate=True;";

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<JobContext>(options =>
    options.UseSqlServer(connectionString));

// Add http clients
builder.Services.AddHttpClient<JobParserService>();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var service = app.Services.CreateScope())
{
    var services = service.ServiceProvider;
    var jobParserService = services.GetRequiredService<JobParserService>();
    var context = services.GetRequiredService<JobContext>();
    // DbSeeder.SeedCategories(context);
    await jobParserService.ParseJobsAsync();
}

app.Run();