
using FixUp.Service.Dto;

using FixUp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixUp.Api.Controllers
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

        // 4. קבלת העבודות שכבר שויכו לבעל המקצוע
        [HttpGet("my-jobs/{profId}")]
        public async Task<ActionResult<IEnumerable<RequestDisplayDto>>> GetMyJobs(int profId)
        {
            var requests = await _requestService.GetMyJobsAsync(profId);
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
    }
}