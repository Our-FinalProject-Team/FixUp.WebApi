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

// 1. הוספת שירותים בסיסיים
builder.Services.AddControllers();
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

builder.Services.AddAuthorization(); // פותר את השגיאה מהחלון השחור
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "FixUp API", Version = "v1" });

    // הגדרת האבטחה - זה מה שחסר לך כדי שהטוקן יעבור!
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// 2. הגדרת Swagger (הקווים האדומים ייעלמו אחרי ההתקנה)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. חיבור ל-SQL (לפי הקוד שלך)
builder.Services.AddDbContext<IContext, DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure())
);


// --- הזרקות ה-Services (התוספת החדשה) ---
builder.Services.AddScoped<IFixUpTaskService, FixUpTaskService>();
builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
builder.Services.AddScoped<IClientService, ClientService>();
//builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();

// הזרקת AutoMapper - חובה כדי שהמרות יעבדו
builder.Services.AddAutoMapper(typeof(MyMapper));



// 4. רישום ה-Repositories (Dependency Injection)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IFixUpTaskRepository, FixUpTaskRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// הוספת מדיניות CORS

// --- חלק השירותים (לפני ה-Build) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("SignalRPolicy");
// 5. הגדרת ה-Pipeline (סדר הפעולות קריטי!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // מחבר את ה-Routes של הקונטרולרים

app.Run(); // הרצה אחת בלבד בסוף

// הוספת UseRouting - חשוב מאוד כדי שה-CORS ידע לאן הבקשה הולכת
app.UseRouting();

// ה-CORS חייב לבוא אחרי ה-Routing ולפני ה-Authorization
app.UseCors("SignalRPolicy");

app.UseAuthorization();

// חיבור ה-Hubs והקונטרולרים
app.MapHub<FixUp.WebAPI.Hubs.ChatHub>("/chatHub");
app.MapControllers();

app.Run();

