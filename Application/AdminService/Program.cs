using AdminService.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure Database
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "admin_schema";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "shared_db_user";
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "shared_db_password";
var connectionString = $"Server={host};Port={port};Database={db};User={user};Password={pass};";

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddHttpClient<AdminService.Services.KeycloakService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var kafkaBootstrap = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092";
builder.Services.AddSingleton<GUIApps.Shared.Services.Kafka.IKafkaProducer>(new GUIApps.Shared.Services.Kafka.KafkaProducer(kafkaBootstrap));
builder.Services.AddHostedService(sp => 
    new AdminService.Services.ReportCreatedConsumer(
        kafkaBootstrap, 
        sp, 
        sp.GetRequiredService<ILogger<AdminService.Services.ReportCreatedConsumer>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-create database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
    dbContext.Database.Migrate();
    
    // Seed initial data
    //DbSeeder.Seed(dbContext);
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
