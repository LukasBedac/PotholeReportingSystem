using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MinIO
var minioEndpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT") ?? "localhost:9000";
var minioAccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY") ?? "minio_user";
var minioSecretKey = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY") ?? "minio_password_123";

// Split the endpoint (e.g. "minio_storage:9000") into host and port
var endpoint = minioEndpoint;
var port = 9000;
if (minioEndpoint.Contains(":"))
{
    var parts = minioEndpoint.Split(':', 2);
    endpoint = parts[0];
    if (int.TryParse(parts[1], out int p))
    {
        port = p;
    }
}

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    //Region is irrelevant (i think)
    return new MinioClient()
        .WithEndpoint(endpoint, port)
        .WithCredentials(minioAccessKey, minioSecretKey)
        .WithSSL(false)
        .WithRegion("us-east-1") // Explicitly set default region to avoid potential 400 mismatch
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
