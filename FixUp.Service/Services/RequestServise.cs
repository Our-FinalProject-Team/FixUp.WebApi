using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requestRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public RequestService(IRequestRepository requestRepository, ICategoryRepository categoryRepository, IMapper mapper)
    {
        _requestRepository = requestRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    // הפונקציה שהוספנו ב-Interface - זו השמירה האמיתית!
    public async Task<RequestDisplayDto> CreateRequestAsync(RequestCreateDto dto)
    {
        var request = _mapper.Map<Request>(dto);
        request.CreatedAt = DateTime.Now;
        request.Status = "חדש";

        // משיכת שם הקטגוריה לפי ה-ID
        var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId);
        if (category != null) request.Subject = category.Name;

        var saved = await _requestRepository.AddAsync(request);
        return _mapper.Map<RequestDisplayDto>(saved);
    }
    public async Task<IEnumerable<RequestDisplayDto>> GetRequestsByClientIdAsync(int clientId)
    {
        var requests = await _requestRepository.GetRequestsByClientIdAsync(clientId);
        return _mapper.Map<IEnumerable<RequestDisplayDto>>(requests);
    }

    public async Task<IEnumerable<RequestDisplayDto>> GetApprovedRequestsByProIdAsync(int proId)
    {
        var requests = await _requestRepository.GetRequestsByProfessionalIdAsync(proId);
        // מחזיר רק את אלו שסטטוס שלהם הוא "מאושר" או "בטיפול"
        var approved = requests.Where(r => r.Status == "מאושר" || r.Status == "בטיפול");
        return _mapper.Map<IEnumerable<RequestDisplayDto>>(approved);
    }

    // מימושים של IService<RequestDisplayDto> - כדי שה-VS לא יכעס
    public async Task AddAsync(RequestDisplayDto item) => throw new NotImplementedException("Use CreateRequestAsync instead");
    public async Task<IEnumerable<RequestDisplayDto>> GetAllAsync() => _mapper.Map<IEnumerable<RequestDisplayDto>>(await _requestRepository.GetAllAsync());
    public async Task<RequestDisplayDto> GetByIdAsync(int id) => _mapper.Map<RequestDisplayDto>(await _requestRepository.GetByIdAsync(id));
    public async Task UpdateAsync(int id, RequestDisplayDto item) { /* מימוש עתידי */ }
    public async Task DeleteAsync(int id) { /* מימוש עתידי */ }

    // שאר הפונקציות שלך...
    public async Task<IEnumerable<RequestDisplayDto>> GetAvailableRequestsForMeAsync(int profId)
    {
        // שליפת כל הבקשות עם סטטוס "חדש" שמשויכות לבעל המקצוע
        var requests = await _requestRepository.GetRequestsByProfessionalIdAsync(profId);
        var pendingRequests = requests.Where(r => r.Status == "חדש");

        return _mapper.Map<IEnumerable<RequestDisplayDto>>(pendingRequests);
    }
    public async Task AddRequestFromDtoAsync(RequestCreateDto dto)
    {
        // 1. מיפוי ה-DTO למודל של בסיס הנתונים
        var newRequest = _mapper.Map<Request>(dto);

        // 2. הוספת ערכי ברירת מחדל
        newRequest.CreatedAt = DateTime.Now;
        newRequest.Status = "חדש";

        // 3. שליפת שם הקטגוריה (בשביל ה-Subject)
        var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId);
        if (category != null)
        {
            newRequest.Subject = category.Name;
        }

        // 4. השמירה האמיתית ב-SQL!
        await _requestRepository.AddAsync(newRequest);

    }

    public async Task<IEnumerable<RequestDisplayDto>> GetMyJobsAsync(int profId)
    {
        // שליפת כל הבקשות שכבר אושרו לבעל המקצוע
        var requests = await _requestRepository.GetRequestsByProfessionalIdAsync(profId);
        var myJobs = requests.Where(r => r.Status == "בטיפול" || r.Status == "מאושר");

        return _mapper.Map<IEnumerable<RequestDisplayDto>>(myJobs);
    }

    // בתוך RequestService.cs
    public async Task<bool> AcceptRequestAsync(int requestId, int profId)
    {
        // בדיקה שהבקשה אכן משויכת לבעל המקצוע
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null || request.ProfessionalId != profId)
            return false;

        // עדכון סטטוס ל'בטיפול'
        await _requestRepository.UpdateStatusAsync(requestId, "בטיפול");
        return true;
    }

    public async Task<bool> UpdateRequestStatusAsync(int requestId, string status)
    {
        try
        {
            Console.WriteLine($"מנסה לעדכן סטטוס לבקשה {requestId} לסטטוס: {status}");

            await _requestRepository.UpdateStatusAsync(requestId, status);

            Console.WriteLine($"הסטטוס עודכן בהצלחה לסטטוס: {status}");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה בעדכון סטטוס: {ex.Message}");
            return false;
        }
    }
    public async Task<IEnumerable<RequestDisplayDto>> GetRequestsByProAsync(int proId)
    {
        // 1. שליפת כל הבקשות מהדאטהבייס ששייכות לבעל המקצוע
        var requests = await _requestRepository.GetRequestsByProfessionalIdAsync(proId);

        // 2. המרה של הנתונים מהטבלה (Entity) לפורמט של התצוגה (DTO)
        return requests.Select(r => new RequestDisplayDto
        {
            Id = r.Id,
            Subject = r.Subject,
            Description = r.Description,
            Address = r.Address,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            ScheduledDate = r.ScheduledDate,
            // כאן אפשר להוסיף שליפת שמות אם יש לך גישה לטבלת המשתמשים
            ClientName = "לקוח " + r.ClientId
        });
    }
}