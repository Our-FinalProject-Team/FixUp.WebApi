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

// --- חלק ה-Pipeline (סדר הפעולות) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// הוספת UseRouting - חשוב מאוד כדי שה-CORS ידע לאן הבקשה הולכת
app.UseRouting();

// ה-CORS חייב לבוא אחרי ה-Routing ולפני ה-Authorization
app.UseCors("SignalRPolicy");

app.UseAuthorization();

// חיבור ה-Hubs והקונטרולרים
app.MapHub<FixUp.WebAPI.Hubs.ChatHub>("/chatHub");
app.MapControllers();

app.Run();




//builder.Services.AddAutoMapper(typeof(MappingProfile));
