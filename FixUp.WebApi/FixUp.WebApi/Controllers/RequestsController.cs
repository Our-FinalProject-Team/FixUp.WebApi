using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        // 1. קבלת כל הבקשות (למנהל מערכת למשל)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetAll()
        {
            var requests = await _requestService.GetAllAsync();
            return Ok(requests);
        }

        // 2. פתיחת בקשה חדשה (מה שהלקוח עושה מהטופס ב-React)
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RequestCreateDto requestDto)
        {
            if (requestDto == null) return BadRequest();

            await _requestService.AddRequestFromDtoAsync(requestDto);
            return Ok(new { message = "הבקשה נוצרה בהצלחה!" });
        }

        // 3. הצ'אט: קבלת בקשות שמתאימות למומחיות של בעל המקצוע
        [HttpGet("available/{profId}")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetAvailable(int profId)
        {
            var requests = await _requestService.GetAvailableRequestsForMeAsync(profId);
            return Ok(requests);
        }

        // 3. קבלת הזמנות ממתינות לאישור עבור בעל מקצוע ספציפי
        [HttpGet("professional/{profId}/pending")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetPendingForProfessional(int profId)
        {
            var requests = await _requestService.GetAvailableRequestsForMeAsync(profId);
            return Ok(requests);
        }

        // 4. קבלת העבודות שכבר שויכו לבעל המקצוע
        [HttpGet("my-jobs/{profId}")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetMyJobs(int profId)
        {
            var requests = await _requestService.GetMyJobsAsync(profId);
            return Ok(requests);
        }

        // 7. קבלת כל ההזמנות של בעל מקצוע
        [HttpGet("professional/{profId}/all")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetAllRequestsForProfessional(int profId)
        {
            var requests = await _requestService.GetRequestsByProAsync(profId);
            return Ok(requests);
        }

        // 5. אישור בקשה על ידי בעל מקצוע (לקיחת העבודה)
        [HttpPut("accept/{requestId}/{profId}")]
        public async Task<ActionResult> AcceptRequest(int requestId, int profId)
        {
            // שים לב שאנחנו שומרים את התוצאה במשתנה success
            bool success = await _requestService.AcceptRequestAsync(requestId, profId);

            if (!success)
            {
                // זה מה שיגרום לסווגר להראות שגיאה אדומה (400) במקום 200
                return BadRequest("לא ניתן לשייך את הבקשה: או שהיא תפוסה, או שהיא לא מתאימה למקצוע שלך.");
            }

            return Ok("הבקשה שויכה בהצלחה!");
        }

        // 6. עדכון סטטוס בקשה (למשל מ'חדש' ל'בטיפול')
        [HttpPut("{requestId}/status")]
        public async Task<ActionResult> UpdateRequestStatus(int requestId, [FromBody] StatusUpdateDto statusDto)
        {
            try
            {
                Console.WriteLine($"מנסה לעדכן סטטוס לבקשה {requestId} לסטטוס: {statusDto.Status}");
                
                bool success = await _requestService.UpdateRequestStatusAsync(requestId, statusDto.Status);
                
                Console.WriteLine($"העדכון הצליח: {success}");
                
                if (!success)
                {
                    return BadRequest("לא ניתן לעדכן את הסטטוס של הבקשה.");
                }

                return Ok("הסטטוס עודכן בהצלחה!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה בעדכון סטטוס: {ex.Message}");
                return BadRequest($"שגיאה: {ex.Message}");
            }

        }
        // לקבלת ההיסטוריה של הלקוח
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetByClient(int clientId)
        {
            var requests = await _requestService.GetRequestsByClientIdAsync(clientId);
            return Ok(requests);
        }

        // לקבלת רק הלקוחות המאושרים של בעל המקצוע
        [HttpGet("professional/{proId}/approved")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetApprovedByPro(int proId)
        {
            var requests = await _requestService.GetApprovedRequestsByProIdAsync(proId);
            return Ok(requests);
        }
    }
}