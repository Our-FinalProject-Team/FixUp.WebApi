using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

namespace FixUp.Service.Services
{
    public class ProfessionalService : IProfessionalService
    {
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IMapper _mapper;

        public ProfessionalService(IProfessionalRepository professionalRepository, IMapper mapper)
        {
            _professionalRepository = professionalRepository;
            _mapper = mapper;
        }

        // מימוש הפונקציות מ-IService:
        public async Task<IEnumerable<ProfessionalDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<ProfessionalDto>>(await _professionalRepository.GetAllProfessionalsAsync());

        public async Task<ProfessionalDto> GetByIdAsync(int id) =>
            _mapper.Map<ProfessionalDto>(await _professionalRepository.GetProfessionalByIdAsync(id));

        public async Task AddAsync(ProfessionalDto item) =>
            await _professionalRepository.AddProfessionalAsync(_mapper.Map<Professional>(item));

        public async Task UpdateAsync(int id, ProfessionalDto item)
        {
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);
            if (prof != null)
            {
                _mapper.Map(item, prof);
                await _professionalRepository.UpdateProfessionalAsync(prof);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _professionalRepository.DeleteProfessionalAsync(id);
        }

        // פונקציית הרישום המיוחדת
        public async Task RegisterProfessionalAsync(ProfessionalDto profDto, string password)
        {
            if (string.IsNullOrEmpty(profDto.Email) || !profDto.Email.Contains("@"))
            {
                throw new Exception("פורמט האימייל אינו תקין (חסר @)");
            }

            // 2. עכשיו בודקים מול ה-Repository אם הוא כבר קיים במסד הנתונים
            bool exists = await _professionalRepository.EmailExistsAsync(profDto.Email);
            if (exists)
            {
                throw new Exception("משתמש עם אימייל זה כבר קיים במערכת");
            }
            var model = _mapper.Map<Professional>(profDto);
            model.PasswordHash = password; // הזרקת סיסמה
            model.CreatedAt = DateTime.Now;
            model.IsDeleted = false;
            await _professionalRepository.AddProfessionalAsync(model);
        }
        public async Task<ProfessionalDto> LoginAsync(string email, string password)
        {
            // השליפה מה-Repository כבר מחזירה רק את אלו ש-IsDeleted == false

            var professionals = await _professionalRepository.GetAllProfessionalsAsync();

            var prof = professionals.FirstOrDefault(p =>
                p.Email == email &&
                p.PasswordHash == password &&
                !p.IsDeleted); // ליתר ביטחון, מוודאים שהוא לא מחוק

            if (prof == null) return null;

            return _mapper.Map<ProfessionalDto>(prof);
        }
        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            // חיפוש המשתמש דרך ה-Repository כדי לקבל את הישות המלאה (Entity)
            var allProfessionals = await _professionalRepository.GetAllProfessionalsAsync();

            // כאן IsDeleted יעבוד כי זו הישות מה-DB ולא ה-Dto
            var pro = allProfessionals.FirstOrDefault(p =>
                p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                p.IsDeleted == false);

            if (pro == null) return false;

            // עדכון השדה הנכון מהמודל שלך
            pro.PasswordHash = newPassword;

            // שליחת האובייקט המלא לפונקציה הקיימת ב-Repository
            await _professionalRepository.UpdateProfessionalAsync(pro);

            return true;
        }
    }
}