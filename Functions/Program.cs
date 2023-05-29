using System.Text;
using Functions.Data.DB;
using Functions.Services;
using Functions.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//ASPNETCORE_AppConfig__TwilioSecret=my-secret
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = new StringBuilder();
connectionString.Append($"server={builder.Configuration["AppConfig:DB:Server"]};");
connectionString.Append($"port={builder.Configuration["AppConfig:DB:Port"]};");
connectionString.Append($"user={builder.Configuration["AppConfig:DB:User"]};");
connectionString.Append($"password={builder.Configuration["AppConfig:DB:Password"]};");
connectionString.Append($"database={builder.Configuration["AppConfig:DB:Database"]};");
Console.WriteLine(connectionString.ToString());

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
builder.Services.AddDbContextFactory<FunctionsContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString.ToString(), serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Debug)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());


builder.Services.AddTransient<IDockerManager,DockerManager>();
builder.Services.AddSingleton<FunctionManager>();
builder.Services.AddHttpClient<IExternalEndpointManager, ExternalEndpointManager>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var dbFactory = app.Services.GetRequiredService<IDbContextFactory<FunctionsContext>>();
var db = await dbFactory.CreateDbContextAsync();
await db.Database.MigrateAsync();


app.Run();