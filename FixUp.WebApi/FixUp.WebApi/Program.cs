
using FixUpSolution.Interfaces;
using FixUpSolution.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FixUp.WebApi_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FixUp.WebApi.Data.DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.
            builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
            builder.Services.AddControllers();
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
