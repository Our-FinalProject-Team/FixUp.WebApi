using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

namespace FixUp.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // 1. מימוש פונקציית האימות (Login)
        public async Task<UserDto> Authenticate(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || user.PasswordHash != loginDto.Password) return null;
            return _mapper.Map<UserDto>(user);
        }

        // 2. מימוש קבלת משתמש לפי ID
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        // 3. מימוש קבלת כל המשתמשים
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        // 4. רישום משתמש חדש
        public async Task RegisterAsync(UserDto userDto, string password)
        {
            var userModel = _mapper.Map<User>(userDto);
            userModel.PasswordHash = password;
            await _userRepository.AddUserAsync(userModel);
            userDto.Id = userModel.Id;
        }

        // 5. מחיקת משתמש
        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        // פונקציות עזר קיימות
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync() => await GetAllAsync();
        public async Task<UserDto> GetUserByIdAsync(int id) => await GetByIdAsync(id);
        public async Task AddUserAsync(UserDto userDto, string password) => await RegisterAsync(userDto, password);

        // מימוש חובה לממשק אם קיים
        public Task UpdateAsync(int id, UserDto item) => throw new NotImplementedException("Update logic not yet implemented");
        public Task AddAsync(UserDto item) => throw new NotImplementedException("Use RegisterAsync");
        public Task AddAsync(UserDto item, string password) => RegisterAsync(item, password);
    }
}