using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FixUp.Service.Dto;

namespace FixUp.Service.Interfaces
{
    public interface IFixUpTaskService : IService<FixUpTaskDto>
    {
        // פונקציה מיוחדת לחישוב מחיר סופי או שינוי סטטוס SOS
        Task<double> CalculateFinalPrice(int taskId);
    }
}