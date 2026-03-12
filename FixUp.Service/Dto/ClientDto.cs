using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FixUp.Service.Dto
{
    public class ClientDto : UserDto
    {
        // רשימת הבקשות של הלקוח
        public List<FixUpTaskDto> MyRequests { get; set; }
    }
}