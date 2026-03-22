using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixUp.Repository.Models;


namespace FixUp.Repository.Interfaces
{
    public interface IRequestRepository
    {
        Task<IEnumerable<Request>> GetAllAsync();
        Task<Request> GetByIdAsync(int id);
        Task<Request> AddAsync(Request request);
        Task UpdateAsync(Request request);
        Task DeleteAsync(int id);
        Task ReleaseRequestsByProfessionalIdAsync(int profId);
        Task UpdateStatusAsync(int id, string status);
        Task<IEnumerable<Request>> GetRequestsByProfessionalIdAsync(int proId);
        Task<IEnumerable<Request>> GetRequestsByClientIdAsync(int clientId);
    }
}