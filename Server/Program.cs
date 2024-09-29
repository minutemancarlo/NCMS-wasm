using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Data.SqlClient;
using System.Data;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Client.Pages.Hotel;
using NCMS_wasm.Server.Services;

var builder = WebApplication.CreateBuilder(args);

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


// Add Dapper Repositories
builder.Services.AddTransient<DeviceRepository>();
builder.Services.AddTransient<CardRepository>();
builder.Services.AddTransient<HotelRepository>();
builder.Services.AddTransient<EventsRepository>();
builder.Services.AddTransient<EmployeeRepository>();
builder.Services.AddTransient<PayslipRepository>();
builder.Services.AddTransient<GasRepository>();

//Add Services
builder.Services.AddScoped<LeaveRequestService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ReceiptService>();

//Add Background Services
builder.Services.AddHostedService<PayslipProcessor>();


// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:Authority"];
    options.Audience = builder.Configuration["Auth0:ApiIdentifier"];
});
// Add services to the container.
builder.Services.AddAuth0AuthenticationClient(config =>
{
    config.Domain = builder.Configuration["Auth0:Authority"];
    config.ClientId = builder.Configuration["Auth0:ClientId"];
    config.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
//var allowedOrigin = builder.Configuration["AllowedOrigin:url"];
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", builder =>
//    {
//        builder.WithOrigins(allowedOrigin) 
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });
//});

builder.Services.AddAuth0ManagementClient().AddManagementAccessToken();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
