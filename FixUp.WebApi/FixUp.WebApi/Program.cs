




using Microsoft.EntityFrameworkCore;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Repositories;
using FixUp.Repository.Data;


namespace FixUp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.
            builder.Services.AddScoped<IProfessionaRepository, ProfessionaRepository>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProfessionaRepository, ProfessionaRepository>();
           builder.Services.AddScoped<IClientRepository, ClientRepository>();
           builder.Services.AddScoped<IFixUpTaskRepository, FixUpTaskRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            FixUpSolution.Models.Professional p = new FixUpSolution.Models.Professional();
            app.Run();
        }
    }
}
