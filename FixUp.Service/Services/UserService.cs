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

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            // המרה מרשימת User לרשימת UserDto
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task AddUserAsync(UserDto userDto, string password)
        {
            // --- לוגיקה עסקית / חישוב ---
            if (password.Length < 6)
            {
                throw new Exception("הסיסמה חייבת להיות לפחות 6 תווים");
            }

            // הופכים את ה-DTO למודל אמיתי כדי לשמור במסד
            var userModel = _mapper.Map<User>(userDto);

            // מעדכנים את הסיסמה (שלא הייתה ב-DTO)
            userModel.PasswordHash = password;

            await _userRepository.AddUserAsync(userModel);
        }

        public Task<UserDto> Authenticate(UserLoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(UserDto item, string password)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, UserDto item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterAsync(UserDto userDto, string password)
        {
            var userModel = _mapper.Map<User>(userDto);
            userModel.PasswordHash = password; // או Password לפי השדה שלך
            await _userRepository.AddUserAsync(userModel);
            userDto.Id = userModel.Id;
        }

        // מימוש ריק או בסיסי ל-AddAsync הגנרי אם צריך
        public Task AddAsync(UserDto item) => throw new NotImplementedException("Use RegisterAsync for users");
    }
}