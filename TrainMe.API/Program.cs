using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TrainMe.Core.Interfaces.Services.Auth;
using TrainMe.Data;
using TrainMe.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<TrainMe.Core.Mapping.MappingProfile>();
});

// Add Repositories
builder.Services.AddScoped<TrainMe.Core.Interfaces.Repositories.User.IUserRepository, TrainMe.Data.Repositories.User.UserRepository>();

// WorkoutItem Repositories
builder.Services.AddScoped<TrainMe.Core.Interfaces.Repositories.WorkoutItem.IWorkoutItemRepository, TrainMe.Data.Repositories.WorkoutItem.WorkoutItemRepository>();
builder.Services.AddScoped<TrainMe.Core.Interfaces.Repositories.WorkoutItem.IWorkoutItemQueryRepository, TrainMe.Data.Repositories.WorkoutItem.WorkoutItemQueryRepository>();
builder.Services.AddScoped<TrainMe.Core.Interfaces.Repositories.WorkoutItem.IWorkoutItemSortRepository, TrainMe.Data.Repositories.WorkoutItem.WorkoutItemSortRepository>();

// RandomExercise Repository
builder.Services.AddScoped<TrainMe.Core.Interfaces.Repositories.IRandomExerciseRepository, TrainMe.Data.Repositories.RandomExerciseRepository>();

// Add Services
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.Auth.IPasswordService, TrainMe.Services.Auth.PasswordService>();
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.Auth.ITokenService, TrainMe.Services.Auth.TokenService>();
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.Auth.IAuthService, TrainMe.Services.Auth.AuthService>();

// WorkoutItem Services
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.WorkoutItem.IWorkoutItemService, TrainMe.Services.WorkoutItem.WorkoutItemService>();
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.WorkoutItem.IWorkoutItemQueryService, TrainMe.Services.WorkoutItem.WorkoutItemQueryService>();
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.WorkoutItem.IWorkoutItemManagementService, TrainMe.Services.WorkoutItem.WorkoutItemManagementService>();

// RandomExercise Service
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.IRandomExerciseService, TrainMe.Services.RandomExerciseService>();

// BMI Service
builder.Services.AddScoped<TrainMe.Core.Interfaces.Services.IBmiService, TrainMe.Services.BmiService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrainMe API", Version = "v1" });
    
    // Cấu hình JWT cho Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// JWT Configuration
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Cho development
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// CORS cho React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrainMe API v1");
        c.RoutePrefix = string.Empty; // Swagger UI ở root
    });
}

// app.UseHttpsRedirection(); // Tắt HTTPS redirect cho development

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

// CORS
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
