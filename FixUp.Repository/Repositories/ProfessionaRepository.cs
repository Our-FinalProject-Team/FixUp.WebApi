
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ProfessionalRepository : IProfessionalRepository
    {
        private readonly IContext _context;
        public ProfessionalRepository(IContext context) => _context = context;

        public async Task<IEnumerable<Professional>> GetAllProfessionalsAsync() =>
    await _context.Professionals.Where(p => !p.IsDeleted).ToListAsync(); // שליפת פעילים בלבד

public async Task<Professional> GetProfessionalByIdAsync(int id) =>
    await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

public async Task<bool> ProfessionalExistsAsync(int id) =>
    await _context.Professionals.AnyAsync(p => p.Id == id && !p.IsDeleted);

        public async Task UpdateProfessionalAsync(Professional professional)
        {
            _context.Professionals.Update(professional);
            await _context.SaveChangesAsync();
        }
        
        public async Task AddProfessionalAsync(Professional professional)
        {
            await _context.Professionals.AddAsync(professional);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProfessionalAsync(int id)
        {
            var prof = await _context.Professionals.FindAsync(id);
            if (prof != null)
            {
                prof.IsDeleted = true; // מחיקה רכה - סימון בלבד
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> EmailExistsAsync(string email) =>
    await _context.Users.AnyAsync(u => u.Email == email);
    }
}