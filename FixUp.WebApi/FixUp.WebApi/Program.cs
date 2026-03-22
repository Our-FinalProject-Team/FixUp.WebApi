using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.Interfaces;
using FixUp.Service.Interfases;
using FixUp.Service.Services;
using FixUpSolution.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. הוספת שירותים (Services) ---

builder.Services.AddControllers();
builder.Services.AddSignalR();

// הגדרת CORS - שים לב: הכתובת כאן חייבת להיות של ה-React (5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // כתובת הפרונטנד שלך
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // חובה עבור SignalR
    });
});

// הגדרת אימות (Authentication)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// הגדרת Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "FixUp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// חיבור ל-DB
builder.Services.AddDbContext<IContext, DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure())
);

// הזרקות (Dependency Injection)
builder.Services.AddAutoMapper(typeof(MyMapper));
builder.Services.AddScoped<IFixUpTaskService, FixUpTaskService>();
builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IFixUpTaskRepository, FixUpTaskRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();

var app = builder.Build();

// --- 2. הגדרת ה-Pipeline (הסדר קריטי!) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// חובה: Routing תמיד לפני CORS
app.UseRouting();

// חובה: CORS תמיד אחרי Routing ולפני Auth
app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

// מיפוי נקודות קצה
app.MapControllers();
app.MapHub<FixUp.WebAPI.Hubs.ChatHub>("/chatHub");

app.Run();