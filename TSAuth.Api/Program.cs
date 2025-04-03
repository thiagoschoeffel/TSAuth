using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TSAuth.Api.Application.Services;
using TSAuth.Api.Infrastructure;
using TSAuth.Api.Models;
using TSAuth.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres") ??
                       throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<TsAuthDbContext>(options =>
    options
        .UseNpgsql(connectionString,
            npgsqlOptions => { npgsqlOptions.MigrationsHistoryTable("ef_migrations_history", schema: "public"); })
        .UseSnakeCaseNamingConvention()
);

var adminSection = builder.Configuration.GetSection("AdministratorUser");
builder.Services.Configure<AdministratorUserSettings>(adminSection);

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);

builder.Services.AddSingleton<AuthService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TsAuthDbContext>();
    var administratorUserSettings =
        scope.ServiceProvider.GetRequiredService<IOptions<AdministratorUserSettings>>().Value;

    dbContext.Database.Migrate();

    if (!dbContext.Users.Any(user => user.Email == administratorUserSettings.Email))
    {
        var user = new User
        {
            Name = administratorUserSettings.Name,
            Email = administratorUserSettings.Email,
            Password = HashService.HashPassword(administratorUserSettings.Password),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TSAuth API v1"); });

app.MapGet("/", () => "TSAuth API is running!");
app.MapControllers();
app.Run();