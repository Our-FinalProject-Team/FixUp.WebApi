using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

namespace FixUp.Service.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepo;
        private readonly IAuthService _authService; // הזרקה חדשה
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepo, IAuthService authService, IMapper mapper)
        {
            _clientRepo = clientRepo;
            _authService = authService;
            _mapper = mapper;
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
            var client = _mapper.Map<Client>(item);
            await _clientRepo.AddClientAsync(client);
        }

        public async Task UpdateAsync(int id, ClientDto item)
        {
            // שליפת הלקוח הקיים מהמסד
            var existingClient = await _clientRepo.GetClientByIdAsync(id);

            if (existingClient != null)
            {
                // שמירת ה-ID המקורי מהמסד כדי למנוע דריסה ל-0 או לערך אחר מה-DTO
                var originalId = existingClient.Id;

                // מיפוי הנתונים החדשים על האובייקט הקיים
                _mapper.Map(item, existingClient);

                // החזרה ידנית של ה-ID המקורי כדי ש-EF לא יזהה ניסיון לשינוי מפתח
                existingClient.Id = originalId;

                // ביצוע העדכון ברפוזיטורי
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
            var clients = await _clientRepo.GetAllClientsAsync();
            var existingClient = clients.FirstOrDefault(c => c.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (existingClient != null && !existingClient.IsDeleted)
                throw new Exception("משתמש פעיל עם אימייל זה כבר קיים");

            // השורה החשובה - הצפנת הסיסמה
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

        // --- לוגין מעודכן עם טוקן ---
        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var clients = await _clientRepo.GetAllClientsAsync();
            var client = clients.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            // בדיקה שהמשתמש קיים, לא מחוק, והסיסמה מתאימה להאש
            if (client == null || client.IsDeleted || !BCrypt.Net.BCrypt.Verify(password, client.PasswordHash))
            {
                return null;
            }

            var clientDto = _mapper.Map<ClientDto>(client);
            var token = _authService.GenerateJwtToken(client.Email, "Client");

            return new AuthResponseDto
            {
                User = clientDto,
                Token = token,
                Role = "Client"
            };
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var allEntities = await _clientRepo.GetAllClientsAsync();
            var user = allEntities.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.IsDeleted == false);

            if (user == null) return false;

            user.PasswordHash = newPassword;
            await _clientRepo.UpdateClientAsync(user);
            return true;
        }
        public async Task UpdateByEmailAsync(string email, ClientDto item)
        {
            var all = await _clientRepo.GetAllClientsAsync();
            var client = all.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (client != null)
            {
                var originalId = client.Id;
                var originalHash = client.PasswordHash; // שומרים על הסיסמה המוצפנת

                _mapper.Map(item, client);

                client.Id = originalId;
                client.PasswordHash = originalHash;

                await _clientRepo.UpdateClientAsync(client);
            }
        }
    }
}