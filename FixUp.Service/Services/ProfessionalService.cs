using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        // פונקציית עזר לבדיקת תקינות - נשארת פרטית ופנימית
        private void ValidateDto(object dto)
        {
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, context, results, true))
            {
                throw new Exception(results.First().ErrorMessage);
            }
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var professionals = await _professionalRepository.GetAllProfessionalsAsync();
            var prof = professionals.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (prof == null || prof.IsDeleted || !BCrypt.Net.BCrypt.Verify(password, prof.PasswordHash))
            {
                return null;
            }

            var profDto = _mapper.Map<ProfessionalDto>(prof);
            var token = _authService.GenerateJwtToken(prof.Email, "Professional", prof.Id);

            return new AuthResponseDto
            {
                User = profDto,
                Token = token,
                Role = "Professional"
            };
        }

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
            ValidateDto(item); // בדיקת תקינות
            var entity = _mapper.Map<Professional>(item);
            await _professionalRepository.AddProfessionalAsync(entity);
        }

        public async Task UpdateAsync(int id, ProfessionalDto item)
        {
            ValidateDto(item); // בדיקת תקינות לפני עדכון
            var prof = await _professionalRepository.GetProfessionalByIdAsync(id);
            if (prof != null)
            {
                var originalId = prof.Id;
                _mapper.Map(item, prof);
                prof.Id = originalId;
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
                await _requestRepository.ReleaseRequestsByProfessionalIdAsync(id);
            }
        }

        public async Task RegisterProfessionalAsync(ProfessionalDto profDto, string password)
        {
            ValidateDto(profDto); // בדיקת השדות ב-DTO

            // בדיקת סיסמה ידנית (כי היא לא בתוך ה-DTO)
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || !password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                throw new Exception("הסיסמה חייבת להיות באורך 6 תווים לפחות ולשלב אותיות ומספרים");

            var professionals = await _professionalRepository.GetAllProfessionalsAsync();
            var existingProf = professionals.FirstOrDefault(p => p.Email.Equals(profDto.Email, StringComparison.OrdinalIgnoreCase));

            if (existingProf != null && !existingProf.IsDeleted)
                throw new Exception("משתמש פעיל עם אימייל זה כבר קיים");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            if (existingProf != null && existingProf.IsDeleted)
            {
                _mapper.Map(profDto, existingProf);
                existingProf.IsDeleted = false;
                existingProf.PasswordHash = passwordHash;
                existingProf.CreatedAt = DateTime.Now;
                await _professionalRepository.UpdateProfessionalAsync(existingProf);
            }
            else
            {
                var newProf = _mapper.Map<Professional>(profDto);
                newProf.PasswordHash = passwordHash;
                newProf.IsDeleted = false;
                await _professionalRepository.AddProfessionalAsync(newProf);
            }
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var pro = all.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (pro == null) return false;

            pro.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            if (pro.IsDeleted) pro.IsDeleted = false;
            await _professionalRepository.UpdateProfessionalAsync(pro);
            return true;
        }

        public async Task UpdateByEmailAsync(string email, ProfessionalDto item)
        {
            ValidateDto(item); // בדיקת תקינות
            var all = await _professionalRepository.GetAllProfessionalsAsync();
            var prof = all.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (prof != null)
            {
                var originalId = prof.Id;
                var originalHash = prof.PasswordHash;
                _mapper.Map(item, prof);
                prof.Id = originalId;
                prof.PasswordHash = originalHash;
                await _professionalRepository.UpdateProfessionalAsync(prof);
            }
        }
    }
}