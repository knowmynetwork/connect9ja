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
var blob = Environment.GetEnvironmentVariable("DatifyAzureBlobStorage");

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
var apiHost = jwtConfig["Issuer"];

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

    /*foreach (var role in roles)
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }*/
}
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.ToLower() == "local")
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
        return Environment.GetEnvironmentVariable("DatifyDevDb");
    else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        return Environment.GetEnvironmentVariable("DatifyProdDb");
    else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
        //return Environment.GetEnvironmentVariable("DatifyLocalDb");
        return Environment.GetEnvironmentVariable("DatifyDevDb");

    else
        return null;
}