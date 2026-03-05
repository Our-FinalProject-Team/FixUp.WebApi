using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Service.Services
{
    public class ProfessionalService : IProfessionalService
    {
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IRequestRepository _requestRepository; // 1. מוסיפים שדה חדש עבור הבקשות
        private readonly IMapper _mapper;

        // 2. מעדכנים את הבנאי (Constructor) שיקבל גם את IRequestRepository
        public ProfessionalService(
            IProfessionalRepository professionalRepository,
            IRequestRepository requestRepository, // הזרקה נוספת
            IMapper mapper)
        {
            _professionalRepository = professionalRepository;
            _requestRepository = requestRepository; // שמירה במשתנה המחלקה
            _mapper = mapper;
        }

        // מימוש הפונקציות מ-IService:
        // מחזיר רק את מי שלא מחוק
        public async Task<IEnumerable<ProfessionalDto>> GetAllAsync()
        {
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var active = all.Where(p => !p.IsDeleted); // סינון ידני לביטחון
            return _mapper.Map<IEnumerable<ProfessionalDto>>(active);
        }

        // מחזיר משתמש ספציפי רק אם הוא לא מחוק
        public async Task<ProfessionalDto> GetByIdAsync(int id)
        {
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);
            if (prof == null || prof.IsDeleted) return null; // אם הוא מחוק, מבחינתנו הוא לא קיים

            return _mapper.Map<ProfessionalDto>(prof);
        }
        public async Task AddAsync(ProfessionalDto item) =>
            await _professionalRepository.AddProfessionalAsync(_mapper.Map<Professional>(item));

        public async Task UpdateAsync(int id, ProfessionalDto item)
        {
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);

            
            if (prof != null && !prof.IsDeleted)
            {
                _mapper.Map(item, prof);
                await _professionalRepository.UpdateProfessionalAsync(prof);
            }
            else
            {
                throw new Exception("לא ניתן לעדכן משתמש מחוק או לא קיים");
            }
        }
        public async Task DeleteAsync(int id)
        {
            // 1. שליפה מה-DB
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);

            if (prof != null)
            {
                // 2. שינוי הסטטוס בזיכרון
                prof.IsDeleted = true;

                // 3. שליחה ל-Repository שיבצע את ה-SaveChangesAsync ל-SQL
                await _professionalRepository.UpdateProfessionalAsync(prof);

                // 4. שחרור הבקשות
                await _requestRepository.ReleaseRequestsByProfessionalIdAsync(id);
            }
        }
        //public async Task DeleteAsync(int id)
        //{
        //    // 1. שולפים את הישות (Entity) מהרפוזיטורי
        //    var prof = await _professionalRepository.GetProfessionalByIdAsync(id);

        //    if (prof != null)
        //    {
        //        // 2. מעדכנים את השדה (כאן זה יעבוד כי prof הוא Entity ולא DTO)
        //        prof.IsDeleted = true;

        //        // 3. שולחים את האובייקט המעודכן חזרה לרפוזיטורי
        //        // פונקציית UpdateProfessionalAsync בתוך הרפוזיטורי כבר כוללת SaveChangesAsync
        //        await _professionalRepository.UpdateProfessionalAsync(prof);

        //        // 4. שחרור הבקשות שלו (באמצעות הרפוזיטורי של הבקשות)
        //        await _requestRepository.ReleaseRequestsByProfessionalIdAsync(id);
        //    }
        //}

        // פונקציית הרישום המיוחדת
        public async Task RegisterProfessionalAsync(ProfessionalDto profDto, string password)
        {
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var existingProf = all.FirstOrDefault(p => p.Email.Equals(profDto.Email, StringComparison.OrdinalIgnoreCase));

            if (existingProf != null)
            {
                if (existingProf.IsDeleted)
                {
                    // --- התיקון כאן ---

                    // 1. נשמור את ה-ID המקורי של המשתמש מה-DB
                    int originalId = existingProf.Id;

                    // 2. נבצע את המיפוי של הפרטים החדשים
                    _mapper.Map(profDto, existingProf);

                    // 3. נוודא שה-ID לא השתנה ל-0, אלא נשאר ה-ID של השורה ב-SQL
                    existingProf.Id = originalId;

                    // 4. עדכון שאר השדות הקריטיים
                    existingProf.IsDeleted = false;
                    existingProf.PasswordHash = password;
                    existingProf.CreatedAt = DateTime.Now;

                    // 5. קריאה לעדכון
                    await _professionalRepository.UpdateProfessionalAsync(existingProf);
                    return;
                }
                else
                {
                    throw new Exception("משתמש פעיל עם אימייל זה כבר קיים");
                }
            }

            // רישום רגיל אם לא קיים...
            var newProf = _mapper.Map<Professional>(profDto);
            newProf.PasswordHash = password;
            newProf.IsDeleted = false;
            await _professionalRepository.AddProfessionalAsync(newProf);
        }
        public async Task<ProfessionalDto> LoginAsync(string email, string password)
        {
            var professionals = await _professionalRepository.GetAllProfessionalsAsync();

            // מחפשים לפי מייל וסיסמה בלבד
            var prof = professionals.FirstOrDefault(p =>
                p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                p.PasswordHash == password);

            if (prof == null) return null;

            // אם המשתמש נמצא אבל הוא היה מסומן כמחוק - מחזירים אותו לפעילות
            if (prof.IsDeleted)
            {
                prof.IsDeleted = false;
                await _professionalRepository.UpdateProfessionalAsync(prof);
            }

            return _mapper.Map<ProfessionalDto>(prof);
        }
        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            // 1. שליפת כל בעלי המקצוע כדי למצוא את המשתמש לפי המייל
            // הערה: כפי שאמרנו קודם, עדיף שתהיה פונקציה ברפוזיטורי ששולפת לפי מייל ישירות
            var allProfessionals = await _professionalRepository.GetAllProfessionalsAsync();

            // 2. חיפוש המשתמש (ללא סינון IsDeleted בשלב החיפוש)
            var pro = allProfessionals.FirstOrDefault(p =>
                p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (pro == null) return false; // המייל לא קיים בכלל במערכת

            // 3. עדכון הסיסמה
            pro.PasswordHash = newPassword;

            // 4. אם המשתמש היה מחוק - מחזירים אותו לפעילות (IsDeleted = false)
            if (pro.IsDeleted)
            {
                pro.IsDeleted = false;
            }

            // 5. שמירת השינויים בבסיס הנתונים
            await _professionalRepository.UpdateProfessionalAsync(pro);

            return true;
        }
    }
}