using AutoMapper;

using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FixUp.Service.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IProfessionalRepository _profRepository;
        private readonly IMapper _mapper;

        public RequestService(IRequestRepository requestRepository, IProfessionalRepository profRepository, IMapper mapper)
        {
            _requestRepository = requestRepository;
            _profRepository = profRepository;
            _mapper = mapper;
        }

        // --- מימוש פונקציות IService הכלליות ---

        public async Task<IEnumerable<RequestDisplayDto>> GetAllAsync()
        {
            var requests = await _requestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RequestDisplayDto>>(requests);
        }

        public async Task<RequestDisplayDto> GetByIdAsync(int id)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            return _mapper.Map<RequestDisplayDto>(request);
        }

        public async Task UpdateAsync(int id, RequestDisplayDto itemDto)
        {
            var existingRequest = await _requestRepository.GetByIdAsync(id);
            if (existingRequest != null)
            {
                // מעדכנים את הישות הקיימת לפי הנתונים מה-DTO
                _mapper.Map(itemDto, existingRequest);
                await _requestRepository.UpdateAsync(existingRequest);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _requestRepository.DeleteAsync(id);
        }

        // פונקציה ריקה כדי לעמוד בתנאי הממשק הכללי (כי אנחנו משתמשים ב-CreateDto במקום)
        public Task AddAsync(RequestDisplayDto item) => throw new System.NotImplementedException("Use AddRequestFromDtoAsync instead");


        // --- פונקציות מיוחדות למערכת FixUp ---

        // 1. יצירת בקשה חדשה מהלקוח
        public async Task AddRequestFromDtoAsync(RequestCreateDto dto)
        {
            var requestEntity = _mapper.Map<Request>(dto);

            requestEntity.CreatedAt = System.DateTime.Now;
            requestEntity.Status = "ממתין"; // סטטוס התחלתי

            await _requestRepository.AddAsync(requestEntity);
        }

        // 2. סינון בקשות עבור הצ'אט/לוח של בעלי מקצוע
        // בתוך RequestService.cs
        public async Task<IEnumerable<RequestDisplayDto>> GetAvailableRequestsForMeAsync(int profId)
        {
            var prof = await _profRepository.GetProfessionalByIdAsync(profId);
            if (prof == null || string.IsNullOrEmpty(prof.Specialty)) return new List<RequestDisplayDto>();

            // 1. פירוק המומחיות למילים (למקרה שכתוב "חשמלאי מוסמך") וניקוי רווחים
            var specialtyKeywords = prof.Specialty
                .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim().ToLower())
                .ToList();

            var allRequests = await _requestRepository.GetAllAsync();

            // 2. סינון חכם
            var filtered = allRequests.Where(r =>
                r.ProfessionalId == null && // רק בקשות שטרם שויכו
                specialtyKeywords.Any(keyword =>
                    (r.Subject != null && r.Subject.ToLower().Contains(keyword)) ||
                    (r.Description != null && r.Description.ToLower().Contains(keyword))
                )
            );

            return _mapper.Map<IEnumerable<RequestDisplayDto>>(filtered);
        }
        // 3. שליפת עבודות שבעל המקצוע כבר "לקח"
        public async Task<IEnumerable<RequestDisplayDto>> GetMyJobsAsync(int profId)
        {
            var all = await _requestRepository.GetAllAsync();
            var myJobs = all.Where(r => r.ProfessionalId == profId);
            return _mapper.Map<IEnumerable<RequestDisplayDto>>(myJobs);
        }
        // בתוך RequestService.cs
        public async Task<bool> AcceptRequestAsync(int requestId, int profId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            var prof = await _profRepository.GetProfessionalByIdAsync(profId);

            // בדיקה שהבקשה קיימת, שבעל המקצוע קיים, ושהבקשה פנויה
            if (request != null && prof != null && request.ProfessionalId == null)
            {
                // בדיקת התאמה מקצועית (Case Insensitive)
                string specialty = prof.Specialty.ToLower().Trim();
                bool isMatch = (request.Subject?.ToLower().Contains(specialty) ?? false) ||
                               (request.Description?.ToLower().Contains(specialty) ?? false);

                if (isMatch)
                {
                    request.ProfessionalId = profId;
                    request.Status = "בטיפול";
                    await _requestRepository.UpdateAsync(request);
                    return true; // רק כאן אנחנו מחזירים אמת
                }
            }

            return false; // בכל מקרה אחר (לא נמצא, תפוס או לא מתאים למקצוע) - נחזיר שקר
        }

    }
}