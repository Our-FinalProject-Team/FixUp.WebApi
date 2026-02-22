using FixUpSolution.Models;

namespace FixUpSolution.Interfaces
{
    public interface IProfessionalRepository
    {
        ICollection<Professional> GetProfessionals();
        Professional GetProfessional(int id);
        bool ProfessionalExists(int id);
        // בהמשך נוסיף כאן גם Create, Update ו-Delete
    }
}