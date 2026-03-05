
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Repository.Repositories
{
    public class ProfessionalRepository : IProfessionalRepository
    {
        private readonly IContext _context;
        public ProfessionalRepository(IContext context) => _context = context;

        // בתוך ProfessionalRepository.cs
        public async Task<IEnumerable<Professional>> GetAllProfessionalsAsync()
        {
            // מחזירים הכל בלי סינון - ה-Service הוא זה שיחליט מה לסנן ללקוח
            return await _context.Professionals.ToListAsync();
        }
        // 1. שליפה לפי ID - תסירי את התנאי !p.IsDeleted כדי שה-Service יוכל "לתפוס" את הישות ולשנות אותה
        public async Task<Professional> GetProfessionalByIdAsync(int id) =>
            await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id); // ללא סינון

        // 2. פונקציית המחיקה בתוך הרפוזיטורי
        public async Task DeleteProfessionalAsync(int id)
        {
            // השתמשי ב-FindAsync - הוא מוצא את השורה ב-SQL בלי קשר לסטטוס שלה
            var prof = await _context.Professionals.FindAsync(id);
            if (prof != null)
            {
                prof.IsDeleted = true;
                await _context.SaveChangesAsync(); // השורה שבאמת מעדכנת את ה-SQL
            }
        }

        public async Task<bool> ProfessionalExistsAsync(int id) =>
    await _context.Professionals.AnyAsync(p => p.Id == id && !p.IsDeleted);

        public async Task UpdateProfessionalAsync(Professional professional)
        {
            // במקום Entry, נשתמש ב-Update שהגדרת. 
            // ה-EF יודע לבד לעקוב אחרי השינויים ברגע שקוראים ל-Update
            _context.Professionals.Update(professional);

            // השורה הכי חשובה - בלי זה שום דבר לא נשלח ל-SQL!
            await _context.SaveChangesAsync();
        }

        public async Task AddProfessionalAsync(Professional professional)
        {
            await _context.Professionals.AddAsync(professional);
            await _context.SaveChangesAsync();
        }

       
        public async Task<bool> EmailExistsAsync(string email) =>
    await _context.Users.AnyAsync(u => u.Email == email);
    }
}