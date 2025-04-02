using System.Collections;
using System.Diagnostics;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Datify.API.Configuration;
using Datify.API.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddAntiforgery(options =>
// {
//     // Optionally configure the header name or other settings
//     //options.HeaderName = "X-XSRF-TOKEN";
//     options.SuppressXFrameOptionsHeader = true;
// });
builder.Services.AddAutoMapper(typeof(MappingProfiles));
var blob = Environment.GetEnvironmentVariable("HomxlyAzureBlobStorage");

builder.Services.AddSingleton(x => new BlobServiceClient(blob));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

var connection = GetConnectionString();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(connection);
            opt.EnableSensitiveDataLogging();
        }
    );


builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ✅ Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ✅ Get the running domain dynamically
var jwtConfig = builder.Configuration.GetSection("Jwt");
var apiHost = "";
if (Debugger.IsAttached)
{
    apiHost = "https://localhost:7035";
}
else
{
    apiHost = "https://proproundwebdev.azurewebsites.net";

    /*using var scope = builder.Services.BuildServiceProvider().CreateScope();
    var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    if (request != null)
    {
        apiHost = $"{request.Scheme}://{request.Host}";
        Console.WriteLine("api host is "+apiHost);
        if (apiHost.EndsWith('/'))
        {
            apiHost = apiHost.Substring(0, apiHost.Length - 1);
            Console.WriteLine("api host is now "+apiHost);

        }
    }

    apiHost = "https://proproundwebdev.azurewebsites.net";*/
}

// ✅ Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // ✅ Disable for local development
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = apiHost, // ✅ Set Issuer dynamically
            ValidAudience = apiHost, // ✅ Set Audience dynamically
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"])),
            ClockSkew = TimeSpan.Zero // ✅ Prevents token expiration delays
        };
    });

builder.Services.AddHttpClient();
builder.Services.AddServices();
builder.Services.AddEndpoints();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User", "GuestUser", "HostUser" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // ✅ Enables detailed error messages

    var scope = app.Services.CreateScope();
    var dbc = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbc?.Database.EnsureCreated();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/identity").MapIdentityApi<ApplicationUser>().WithTags("Identity");
//app.UseAntiforgery();

app.UseEndpoints();

app.Run();

static string? GetConnectionString()
{
    foreach (DictionaryEntry env in Environment.GetEnvironmentVariables()) Console.WriteLine($"{env.Key}: {env.Value}");
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        return Environment.GetEnvironmentVariable("HomxlyDevDb3");
    else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        return Environment.GetEnvironmentVariable("HomxlyProdDb");
    else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
        return Environment.GetEnvironmentVariable("HomxlyLocalDb3");
    else
        return null;
}