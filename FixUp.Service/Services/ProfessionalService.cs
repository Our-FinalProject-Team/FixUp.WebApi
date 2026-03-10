using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FixUp.Service.Services
{
    public class ProfessionalService : IProfessionalService
    {
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public ProfessionalService(
            IProfessionalRepository professionalRepository,
            IRequestRepository requestRepository,
            IAuthService authService,
            IMapper mapper)
        {
            _professionalRepository = professionalRepository;
            _requestRepository = requestRepository;
            _authService = authService;
            _mapper = mapper;
        }

        // --- לוגין מעודכן עם טוקן ---
        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var professionals = await _professionalRepository.GetAllProfessionalsAsync();

            // מוצאים את המשתמש לפי אימייל בלבד (כי את הסיסמה אי אפשר לשלוף בשאילתה)
            var prof = professionals.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            // בודקים: האם המשתמש קיים? האם הוא לא מחוק? והאם הסיסמה שהזין מתאימה להאש שבמסד?
            if (prof == null || prof.IsDeleted || !BCrypt.Net.BCrypt.Verify(password, prof.PasswordHash))
            {
                return null; // אימות נכשל
            }

            var profDto = _mapper.Map<ProfessionalDto>(prof);
            var token = _authService.GenerateJwtToken(prof.Email, "Professional");

            return new AuthResponseDto
            {
                User = profDto,
                Token = token,
                Role = "Professional"
            };
        }
        // --- שאר הפונקציות המקוריות שלך ללא שינוי לוגי ---
        public async Task<IEnumerable<ProfessionalDto>> GetAllAsync()
        {
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var active = all.Where(p => !p.IsDeleted);
            return _mapper.Map<IEnumerable<ProfessionalDto>>(active);
        }

        public async Task<ProfessionalDto> GetByIdAsync(int id)
        {
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);
            if (prof == null || prof.IsDeleted) return null;
            return _mapper.Map<ProfessionalDto>(prof);
        }

        public async Task AddAsync(ProfessionalDto item)
        {
            var entity = _mapper.Map<Professional>(item);
            await _professionalRepository.AddProfessionalAsync(entity);
        }

        public async Task UpdateAsync(int id, ProfessionalDto item)
        {
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);
            if (prof != null)
            {
                var originalId = prof.Id; // שמירת ה-ID מה-DB
                _mapper.Map(item, prof);
                prof.Id = originalId;     // החזרה של ה-ID המקורי

                await _professionalRepository.UpdateProfessionalAsync(prof);
            }
        }
        public async Task DeleteAsync(int id)
        {
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);
            if (prof != null)
            {
                prof.IsDeleted = true;
                await _professionalRepository.UpdateProfessionalAsync(prof);
                // שחרור בקשות (לוגיקה מקורית שלך)
                await _requestRepository.ReleaseRequestsByProfessionalIdAsync(id);
            }
        }
        public async Task RegisterProfessionalAsync(ProfessionalDto profDto, string password)
        {
            var professionals = await _professionalRepository.GetAllProfessionalsAsync();
            var existingProf = professionals.FirstOrDefault(p => p.Email.Equals(profDto.Email, StringComparison.OrdinalIgnoreCase));

            if (existingProf != null && !existingProf.IsDeleted)
                throw new Exception("Active user with this email already exists");

            // --- השורה החדשה והחשובה ---
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            // ---------------------------

            if (existingProf != null && existingProf.IsDeleted)
            {
                _mapper.Map(profDto, existingProf);
                existingProf.IsDeleted = false;
                existingProf.PasswordHash = passwordHash; // שמירת ההאש
                existingProf.CreatedAt = DateTime.Now;
                await _professionalRepository.UpdateProfessionalAsync(existingProf);
            }
            else
            {
                var newProf = _mapper.Map<Professional>(profDto);
                newProf.PasswordHash = passwordHash; // שמירת ההאש
                newProf.IsDeleted = false;
                await _professionalRepository.AddProfessionalAsync(newProf);
            }
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var pro = all.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (pro == null) return false;

            // הצפנה מחדש של הסיסמה החדשה
            pro.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (pro.IsDeleted) pro.IsDeleted = false;
            await _professionalRepository.UpdateProfessionalAsync(pro);
            return true;
        }
        public async Task UpdateByEmailAsync(string email, ProfessionalDto item)
        {
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var prof = all.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (prof != null)
            {
                var originalId = prof.Id;
                var originalHash = prof.PasswordHash; // שומרים על הסיסמה שלא תימחק

                _mapper.Map(item, prof);

                prof.Id = originalId;
                prof.PasswordHash = originalHash;

                await _professionalRepository.UpdateProfessionalAsync(prof);
            }
        }

    }
}