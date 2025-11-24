using System.Data;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<VibeLoopBE.Filters.GlobalExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow all origins (for frontend integration)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => new SqliteConnection(connectionString));

// Register repositories
builder.Services.AddScoped<VibeLoopBE.Repositories.IMemberRepository, VibeLoopBE.Repositories.MemberRepository>();
builder.Services.AddScoped<VibeLoopBE.Repositories.IGoalRepository, VibeLoopBE.Repositories.GoalRepository>();

// Register services
builder.Services.AddScoped<VibeLoopBE.Services.IMemberService, VibeLoopBE.Services.MemberService>();
builder.Services.AddScoped<VibeLoopBE.Services.IGoalService, VibeLoopBE.Services.GoalService>();

var app = builder.Build();

// Initialize database (run schema and seed data)
await InitializeDatabaseAsync(connectionString!);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Database initialization helper
async Task InitializeDatabaseAsync(string connString)
{
    using var connection = new SqliteConnection(connString);
    await connection.OpenAsync();

    // Run schema.sql
    var schemaPath = Path.Combine(AppContext.BaseDirectory, "Data", "schema.sql");
    if (File.Exists(schemaPath))
    {
        var schemaSql = await File.ReadAllTextAsync(schemaPath);
        using var schemaCommand = connection.CreateCommand();
        schemaCommand.CommandText = schemaSql;
        await schemaCommand.ExecuteNonQueryAsync();
    }

    // Run seed.sql
    var seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "seed.sql");
    if (File.Exists(seedPath))
    {
        var seedSql = await File.ReadAllTextAsync(seedPath);
        using var seedCommand = connection.CreateCommand();
        seedCommand.CommandText = seedSql;
        await seedCommand.ExecuteNonQueryAsync();
    }
}
