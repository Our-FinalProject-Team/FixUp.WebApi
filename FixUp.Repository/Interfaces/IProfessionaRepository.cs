using FixUpSolution.Models;
namespace FixUp.Repository.Interfaces
{
    public interface IProfessionaRepository
    {
        ICollection<Professional> GetProfessionals();
        Professional GetProfessional(int id);
        bool ProfessionalExists(int id);
        // בהמשך נוסיף כאן גם Create, Update ו-Delete
    }
}