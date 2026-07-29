using Employee.Core.Abstraction;
using Employee.Core.Exceptions;
using Employee.Core.Implementation;
using Employee.Core.Mappings;
using Employee.Domain;
using Employee.Infrastructure.Implementation;
using Employee.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(@"C:\Logs\log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<Employee.Domain.Repositories.IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<Employee.Domain.Repositories.IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<EmployeeProfile>());

builder.Services.AddMemoryCache();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key is missing in configuration.");

    var signingKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtKey));

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = signingKey
    };
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(
                new
                {
                    statusCode = 401,
                    message = "Your session has expired. Please login again"
                });
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.HttpContext.Items["TokenExpired"] = true;
            }

            return Task.CompletedTask;
        },
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Employee Api",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter JWT token"
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
    var exceptionHandlerFeature = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error;
    // Log the exception or perform any necessary actions here
    var statusCode = exception switch
    {
        BadRequestException => 400,
        UnauthorizedException => 401,
        NotFoundException => 404,
        ConflictException => 409,
        _ => 500
    };

    return Results.Json(
        new
        {
            statusCode = statusCode,
            message = exception?.Message
        },
        statusCode: statusCode
    );
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
