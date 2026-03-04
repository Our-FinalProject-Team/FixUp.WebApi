using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixUp.Service.Dto;
namespace FixUp.Service.Interfaces
{
   
   

    public interface IRequestService : IService<RequestDisplayDto>
    {
        // פונקציות ייחודיות לבקשות שלא קיימות ב-IService הכללי
        Task<IEnumerable<RequestDisplayDto>> GetAvailableRequestsForMeAsync(int profId);
        Task<IEnumerable<RequestDisplayDto>> GetMyJobsAsync(int profId);
        Task AddRequestFromDtoAsync(RequestCreateDto dto);
        Task<bool> AcceptRequestAsync(int requestId, int profId);
    }
}
