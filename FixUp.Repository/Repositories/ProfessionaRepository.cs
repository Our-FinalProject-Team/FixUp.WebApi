
using FixUp.Repository.Interfaces;

using FixUp.Repository.Models;

namespace FixUp.Repository.Repositories
{
    public class ProfessionaRepository : IProfessionaRepository
    {
        private readonly IContext _context;

        public ProfessionaRepository(IContext context)
        {
            _context = context;
        }

        public ICollection<Professional> GetProfessionals()
        {
            return _context.Professionals.OrderBy(p => p.Id).ToList();
        }

        public Professional GetProfessional(int id)
        {
            return _context.Professionals.Where(p => p.Id == id).FirstOrDefault();
        }

        public bool ProfessionalExists(int id)
        {
            return _context.Professionals.Any(p => p.Id == id);
        }
    }
}