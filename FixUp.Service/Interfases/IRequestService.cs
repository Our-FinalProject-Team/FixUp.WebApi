using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixUp.Service.Dto;

namespace FixUp.Service.Interfaces
{
    public interface IRequestService : IService<RequestDisplayDto>
    {
        // זו הפונקציה שהקונטרולר יקרא לה
        Task<RequestDisplayDto> CreateRequestAsync(RequestCreateDto dto);

        Task<IEnumerable<RequestDisplayDto>> GetAvailableRequestsForMeAsync(int profId);
        Task<IEnumerable<RequestDisplayDto>> GetMyJobsAsync(int profId);
        Task<bool> AcceptRequestAsync(int requestId, int profId);
        Task AddRequestFromDtoAsync(RequestCreateDto dto);
        Task<bool> UpdateRequestStatusAsync(int requestId, string status);
        Task<IEnumerable<RequestDisplayDto>> GetRequestsByProAsync(int proId);
        Task<IEnumerable<RequestDisplayDto>> GetRequestsByClientIdAsync(int clientId);
        Task<IEnumerable<RequestDisplayDto>> GetApprovedRequestsByProIdAsync(int proId);
    }
}