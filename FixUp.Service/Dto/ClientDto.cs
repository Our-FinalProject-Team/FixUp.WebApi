using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixUp.Service.Dto
{
    public class ClientDto : UserDto
    {
        // כאן יבואו שדות שייחודיים רק ללקוח, למשל:
         public List<FixUpTaskDto> MyRequests { get; set; }
    }
}
