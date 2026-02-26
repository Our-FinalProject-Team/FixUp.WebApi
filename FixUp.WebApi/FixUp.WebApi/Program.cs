using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.Interfaces;
using FixUp.Service.Services;
using FixUpSolution.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. הוספת שירותים בסיסיים
builder.Services.AddControllers();
builder.Services.AddAuthorization(); // פותר את השגיאה מהחלון השחור

// 2. הגדרת Swagger (הקווים האדומים ייעלמו אחרי ההתקנה)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. חיבור ל-SQL (לפי הקוד שלך)
builder.Services.AddDbContext<IContext, DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. רישום ה-Repositories (Dependency Injection)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IFixUpTaskRepository, FixUpTaskRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

// 5. הגדרת ה-Pipeline (סדר הפעולות קריטי!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // כאן יופיע הממשק הגרפי
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // מחבר את ה-Routes של הקונטרולרים

app.Run(); // הרצה אחת בלבד בסוף





// Services
builder.Services.AddScoped<IFixUpTaskService, FixUpTaskService>();
//builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
//builder.Services.AddScoped<IClientService, ClientService>();
//builder.Services.AddScoped<IReviewService, ReviewService>();

// --- הזרקות ה-Services (התוספת החדשה) ---
builder.Services.AddScoped<IUserService, UserService>();

// הזרקת AutoMapper - חובה כדי שהמרות יעבדו
builder.Services.AddAutoMapper(typeof(MyMapper));

builder.Services.AddControllers();
//builder.Services.AddAutoMapper(typeof(MappingProfile));
//builder.Services.AddScoped<IUserService, UserService>();