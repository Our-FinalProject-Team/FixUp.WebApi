using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using FixUp.Service.Interfases;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requestRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IClientRepository _clientRepository;
    private readonly IProfessionalRepository _professionalRepository;

    public RequestService(IRequestRepository requestRepository, ICategoryRepository categoryRepository, IMapper mapper, IEmailService emailService, IClientRepository clientRepository, IProfessionalRepository professionalRepository)
    {
        _requestRepository = requestRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _emailService = emailService;
        _clientRepository = clientRepository;
        _professionalRepository = professionalRepository;
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

    
    public async Task<IEnumerable<RequestDisplayDto>> GetAllAsync()
    {//שליפת כל הבקשות מהמסד והמרתן למבנה שמתאים לריאקט
        var requests = await _requestRepository.GetAllAsync();//שליפה מהמסד
        return _mapper.Map<IEnumerable<RequestDisplayDto>>(requests);//המרה ל DTO והחזרה.
    }

    public async Task<RequestDisplayDto> GetByIdAsync(int id)
    {//שליפת בקשה ספציפית לפי מזהה
        var request = await _requestRepository.GetByIdAsync(id);//מציאת הבקשה
        return _mapper.Map<RequestDisplayDto>(request);//המרה ל DTO והחזרה
    }

    public async Task UpdateAsync(int id, RequestDisplayDto itemDto)
    {//עדכון פרטים של בקשה קיימת
        var existingRequest = await _requestRepository.GetByIdAsync(id);//חיפוש הבקשה בדטה
        if (existingRequest != null)
        {
            // מעדכנים את הישות הקיימת לפי הנתונים מה-DTO
            _mapper.Map(itemDto, existingRequest);//המפר לוקח את הנתונים החדשים מDTO ומעתיקם ליישות הקיימת
            await _requestRepository.UpdateAsync(existingRequest);//שמירת הנתונים במסד
        }
    }

    public async Task DeleteAsync(int id)
    {//מחיקת בקשת לפי מזהה מהמערכת
        await _requestRepository.DeleteAsync(id);//שמירת הנתונים במסד
    }

    public Task AddAsync(RequestDisplayDto item) => throw new System.NotImplementedException("Use AddRequestFromDtoAsync instead");

    // שאר הפונקציות שלך...
    public async Task<IEnumerable<RequestDisplayDto>> GetAvailableRequestsForMeAsync(int profId)
    {
        var prof = await _professionalRepository.GetProfessionalByIdAsync(profId);
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
    public async Task AddRequestFromDtoAsync(RequestCreateDto dto)
    {
        var requestEntity = _mapper.Map<Request>(dto);//מעתיק את הנתונים שבאו מהשרת (DTO) לשורה שתתאים לדטה (Entity)
        requestEntity.CreatedAt = System.DateTime.Now;//תאריך יצירת הבקשה 
        requestEntity.Status = "ממתין";//סטטוס ממתין (לאישור בעל המקצוע
        requestEntity.ApprovalToken = Guid.NewGuid().ToString();//קוד יחודי לבקשה זו

        await _requestRepository.AddAsync(requestEntity);//שליחת הנתונים ל SQL
        string acceptUrl = $"http://localhost:5208/api/Requests/accept-request/{requestEntity.Id}/{requestEntity.ProfessionalId}";//קישור אישור לבעל מקצוע
        string siteUrl = "http://localhost:5173/my-requests";//קישור לאתר ללקוח
                                                             //שליחת מייל ללקוח
                string body = $@"
                    <div style='direction: rtl; font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden; background-color: white;'>
                        <div style='background: linear-gradient(135deg, #FFB900 0%, #28A745 100%); color: white; padding: 25px; text-align: center;'>
                            <h1 style='margin: 0;'>FIXUP</h1>
                        </div>
                        <div style='padding: 25px; line-height: 1.6; color: #333;'>
                            <h2 style='color: #28A745;'>שלום {dto.CustomerName},</h2>
                            <p>קיבלנו את בקשתך בנושא: <strong style='color: #FFB900;'>{dto.Subject}</strong>.</p>
                            <div style='background-color: #f1f9f2; padding: 15px; border-right: 5px solid #28A745; margin: 20px 0;'>
                                {dto.Description}
                            </div>
                            <p>תוכל לעקוב אחר הבקשה בקישור הבא: <a href='{siteUrl}'>{siteUrl}</a></p>
                            <p>ברגע שבעל מקצוע יאשר את הבקשה, נשלח לך עדכון נוסף.</p>
                        </div>
                    </div>";
                await _emailService.SendEmailAsync(
                dto.CustomerEmail,
                "בקשתך התקבלה ב-FixUp",
                body);
                //שליחת מייל לבעל המקצוע שיאשר את הבקשה
                var prof = await _professionalRepository.GetProfessionalByIdAsync(dto.ProfessionalId);
                string displayName = prof?.FullName ?? "בעל מקצוע יקר";//prof.FullName
                                                                       //if (prof != null)
                                                                       //{
                string body_prof = $@"
                    <div style='direction: rtl; font-family: Segoe UI, Arial; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 15px; padding: 20px; background-color: #f9f9f9;'>
                        <h2 style='color: #28A745; text-align: center;'>הזמנת עבודה חדשה מחכה לך!</h2>
    
                        <div style='background-color: white; padding: 15px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                            <p><strong>🛠️ מהות העבודה:</strong> {dto.Subject}</p>
                            <p><strong>👤 שם הלקוח:</strong> {dto.CustomerName}</p>
                            <p><strong>📍 מיקום/פרטים נוספים:</strong> {dto.Description}</p>
                        </div>

                        <div style='text-align: center; margin-top: 25px;'>
                            <a href='{acceptUrl}' style='background-color: #28A745; color: white; padding: 12px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 18px; display: inline-block;'>
                                לצפייה בפרטים המלאים ואישור העבודה 
                            </a>
                        </div>
                    </div>";

        var profEmail = await _professionalRepository.GetProfessionalByIdAsync(dto.ProfessionalId);
        await _emailService.SendEmailAsync(profEmail.Email, "הצעת עבודה חדשה!", body_prof);
        //await _emailService.SendEmailAsync(prof.Email, "הצעת עבודה חדשה!", body_prof);
        // }           

    }

    //public async Task<IEnumerable<RequestDisplayDto>> GetMyJobsAsync(int profId)
    //{
    //    var all = await _requestRepository.GetAllAsync();
    //    var myJobs = all.Where(r => r.ProfessionalId == profId);
    //    return _mapper.Map<IEnumerable<RequestDisplayDto>>(myJobs);
    //}

    public async Task<IEnumerable<RequestDisplayDto>> GetMyJobsAsync(int profId)
    {
        // שליפת כל הבקשות שכבר אושרו לבעל המקצוע
        var requests = await _requestRepository.GetRequestsByProfessionalIdAsync(profId);
        var myJobs = requests.Where(r => r.Status == "בטיפול" || r.Status == "מאושר");

        return _mapper.Map<IEnumerable<RequestDisplayDto>>(myJobs);
    }

    public async Task<bool> AcceptRequestAsync(int requestId, int profId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        var prof = await _professionalRepository.GetProfessionalByIdAsync(profId);

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
                //שליפת נתוני לקוח
                var client = await _clientRepository.GetClientByIdAsync(request.ClientId);
                string coustomerMail = client.Email;
                string customerName = client.FullName;
                string siteUrl = "http://localhost:5173/my-requests";
                string body = $@"
                    <div style='direction: rtl; font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden; background-color: white;'>
                        <div style='background: #28A745; color: white; padding: 20px; text-align: center;'>
                            <h1 style='margin: 0;'>נמצא לך בעל מקצוע! ✨</h1>
                        </div>
                        <div style='padding: 25px; line-height: 1.6; color: #333;'>
                            <h2 style='color: #28A745;'>שלום {customerName},</h2>
                            <p>חדשות טובות! בעל המקצוע <strong>{prof.FullName}</strong> אישר את בקשתך בנושא <strong>{request.Subject}</strong>.</p>
                            <p>הוא יצור איתך קשר בהקדם לתיאום סופי.</p>
        
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{siteUrl}' style='background-color: #FFB900; color: #333; padding: 15px 25px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 18px;'>
                                    לצפייה בפרטי בעל המקצוע והצ'אט
                                </a>
                            </div>

                            <p style='font-size: 14px; color: #888;'>קוד האישור הסודי שלך מול הטכנאי: <strong>{request.ApprovalToken.Substring(0, 5)}</strong></p>
                        </div>
                    </div>";
                await _emailService.SendEmailAsync(coustomerMail, "!מעדכנים,נמצא לך בעל מקצוע", body);
                return true; 
            }
        }

        return false; 
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