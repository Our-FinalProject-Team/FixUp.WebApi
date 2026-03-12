using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FixUp.Service.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepo;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepo, IAuthService authService, IMapper mapper)
        {
            _clientRepo = clientRepo;
            _authService = authService;
            _mapper = mapper;
        }

        // פונקציית עזר לבדיקת תקינות
        private void ValidateDto(object dto)
        {
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, context, results, true))
            {
                throw new Exception(results.First().ErrorMessage);
            }
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync()
        {
            var clients = await _clientRepo.GetAllClientsAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto> GetByIdAsync(int id)
        {
            var client = await _clientRepo.GetClientByIdAsync(id);
            return _mapper.Map<ClientDto>(client);
        }

        public async Task AddAsync(ClientDto item)
        {
            ValidateDto(item); // בדיקת תקינות
            var client = _mapper.Map<Client>(item);
            await _clientRepo.AddClientAsync(client);
        }

        public async Task UpdateAsync(int id, ClientDto item)
        {
            ValidateDto(item); // בדיקת תקינות
            var existingClient = await _clientRepo.GetClientByIdAsync(id);
            if (existingClient != null)
            {
                var originalId = existingClient.Id;
                var originalHash = existingClient.PasswordHash;

                _mapper.Map(item, existingClient);

                existingClient.Id = originalId;
                existingClient.PasswordHash = originalHash;

                await _clientRepo.UpdateClientAsync(existingClient);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _clientRepo.GetClientByIdAsync(id);
            if (client != null)
            {
                client.IsDeleted = true;
                await _clientRepo.UpdateClientAsync(client);
            }
        }

        public async Task RegisterClientAsync(ClientDto dto, string password)
        {
            ValidateDto(dto); // בדיקת DTO

            // בדיקת סיסמה
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || !password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                throw new Exception("הסיסמה חייבת להכיל אותיות ומספרים (מינימום 6 תווים)");

            var clients = await _clientRepo.GetAllClientsAsync();
            var existingClient = clients.FirstOrDefault(c => c.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (existingClient != null && !existingClient.IsDeleted)
                throw new Exception("משתמש פעיל עם אימייל זה כבר קיים");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            if (existingClient != null && existingClient.IsDeleted)
            {
                _mapper.Map(dto, existingClient);
                existingClient.IsDeleted = false;
                existingClient.PasswordHash = passwordHash;
                await _clientRepo.UpdateClientAsync(existingClient);
            }
            else
            {
                var clientModel = _mapper.Map<Client>(dto);
                clientModel.PasswordHash = passwordHash;
                clientModel.IsDeleted = false;
                await _clientRepo.AddClientAsync(clientModel);
            }
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var clients = await _clientRepo.GetAllClientsAsync();
            var client = clients.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (client == null || client.IsDeleted || !BCrypt.Net.BCrypt.Verify(password, client.PasswordHash))
                return null;

            var clientDto = _mapper.Map<ClientDto>(client);
            var token = _authService.GenerateJwtToken(client.Email, "Client");

            return new AuthResponseDto { User = clientDto, Token = token, Role = "Client" };
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var allEntities = await _clientRepo.GetAllClientsAsync();
            var user = allEntities.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && !u.IsDeleted);

            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _clientRepo.UpdateClientAsync(user);
            return true;
        }

        public async Task UpdateByEmailAsync(string email, ClientDto item)
        {
            ValidateDto(item); // בדיקת תקינות
            var all = await _clientRepo.GetAllClientsAsync();
            var client = all.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (client != null)
            {
                var originalId = client.Id;
                var originalHash = client.PasswordHash;
                _mapper.Map(item, client);
                client.Id = originalId;
                client.PasswordHash = originalHash;
                await _clientRepo.UpdateClientAsync(client);
            }
        }
    }
}