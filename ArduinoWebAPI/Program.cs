using ArduinoWebAPI.Repository;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Configure environment-specific settings
var environment = builder.Environment;
if (!string.IsNullOrWhiteSpace(environment.EnvironmentName))
{
    builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", false, true);
}
else
{
    builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
       .AddJsonFile($"appsettings.Production.json", false, true);
}

// Register your database connection
builder.Services.AddTransient<IDbConnection>(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
    string connectionString = configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});

// Add repositories
builder.Services.AddTransient<DeviceRepository>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()  // Allow any origin
               .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
               .AllowAnyHeader(); // Allow any header
    });

    // You can also define specific origins like this:
    /*
    options.AddPolicy("MySpecificPolicy", builder =>
    {
        builder.WithOrigins("http://example.com", "http://anotherexample.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
    */
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAllOrigins"); // Use the CORS policy here

app.UseAuthorization();

app.MapControllers();

app.Run();
