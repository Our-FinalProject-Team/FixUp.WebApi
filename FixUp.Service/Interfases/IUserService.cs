using FixUp.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUserService : IService<UserDto>
{
    // פונקציה מיוחדת לרישום - רק כאן יש סיסמה!
    Task RegisterAsync(UserDto userDto, string password);
    Task<UserDto> Authenticate(UserLoginDto loginDto);
}
