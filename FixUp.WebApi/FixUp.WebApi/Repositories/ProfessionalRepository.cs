using FixUp.WebApi.Data; // ודאי שה-Namespace כאן תקין לפי המקום שבו נמצא ה-DataContext
using FixUpSolution.Interfaces;
using FixUpSolution.Models;

namespace FixUpSolution.Repositories
{
    public class ProfessionalRepository : IProfessionalRepository
    {
        private readonly DataContext _context;

        public ProfessionalRepository(DataContext context)
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