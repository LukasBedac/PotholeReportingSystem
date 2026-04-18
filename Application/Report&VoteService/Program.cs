using Microsoft.EntityFrameworkCore;
using ReportVoteService.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//This was added beacuse Report contains minioFile and minioFile contains Report -> it cycled
//But the save action was OK, only jsonDeserialization crashed
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//TODO 1: Create config for this / secret storage
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "reports_schema";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "shared_db_user";
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "shared_db_password";
var connectionString = $"Server={host};Port={port};Database={db};User={user};Password={pass};";

builder.Services.AddDbContext<ReportsVoteDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddHttpClient("FileService", client =>
{
    client.BaseAddress = new Uri("http://filestorage-service:8080");
});

builder.Services.AddHttpClient("AdminService", client =>
{
    client.BaseAddress = new Uri("http://admin-service:8080");
});

var kafkaBootstrap = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092";
builder.Services.AddSingleton<GUIApps.Shared.Services.Kafka.IKafkaProducer>(new GUIApps.Shared.Services.Kafka.KafkaProducer(kafkaBootstrap));
builder.Services.AddHostedService(sp => 
    new ReportVoteService.Services.AdminAssignedConsumer(
        kafkaBootstrap, 
        sp, 
        sp.GetRequiredService<ILogger<ReportVoteService.Services.AdminAssignedConsumer>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ReportsVoteDbContext>();
    // This will create the database if it doesn't exist and apply pending migrations
    dbContext.Database.Migrate();

    // Seed initial data
    await ReportVoteService.Database.DbSeeder.SeedAsync(dbContext, scope.ServiceProvider.GetRequiredService<IHttpClientFactory>());
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
