
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
            await _context.Professionals.ToListAsync();

        public async Task<Professional> GetProfessionalByIdAsync(int id) =>
            await _context.Professionals.FindAsync(id);

        public async Task<bool> ProfessionalExistsAsync(int id) =>
            await _context.Professionals.AnyAsync(p => p.Id == id);

        public async Task UpdateProfessionalAsync(Professional professional)
        {
            _context.Professionals.Update(professional);
            await _context.SaveChangesAsync();
        }
    }
}