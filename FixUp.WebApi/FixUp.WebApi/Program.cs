using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.Interfaces;
using FixUp.Service.Interfases;
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
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
// הוספת מדיניות CORS

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors("AllowAll");
// 5. הגדרת ה-Pipeline (סדר הפעולות קריטי!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // כאן יופיע הממשק הגרפי
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHub<FixUp.WebAPI.Hubs.ChatHub>("/chatHub");

app.MapControllers(); // מחבר את ה-Routes של הקונטרולרים

app.Run(); // הרצה אחת בלבד בסוף






//builder.Services.AddAutoMapper(typeof(MappingProfile));
