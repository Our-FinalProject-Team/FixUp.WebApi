using FixUp.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUserService : IService<UserDto>
{
    Task<UserDto> Authenticate(UserLoginDto loginDto);
    Task RegisterAsync(UserDto userDto, string password);

}
