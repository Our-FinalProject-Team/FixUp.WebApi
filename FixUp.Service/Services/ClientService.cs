using AutoMapper;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.Dto;
using FixUp.Service.Interfaces;

namespace FixUp.Service.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepo;
        private readonly IMapper _mapper;

        public ClientService(IClientRepository clientRepo, IMapper mapper)
        {
            _clientRepo = clientRepo;
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
            await _clientRepo.UpdateClientAsync(client); // או AddClientAsync אם קיים ב-Repo
        }

        public async Task UpdateAsync(int id, ClientDto item)
        {
            var existingClient = await _clientRepo.GetClientByIdAsync(id);
            if (existingClient != null)
            {
                _mapper.Map(item, existingClient);
                await _clientRepo.UpdateClientAsync(existingClient);
            }
        }

        public async Task DeleteAsync(int id)
        {
            // מימוש מחיקה דרך ה-Repository
            await _clientRepo.DeleteClientAsync(id);
        }

        public async Task RegisterClientAsync(ClientDto clientDto, string password)
        {
            // המרה מ-DTO למודל Client
            var clientModel = _mapper.Map<Client>(clientDto);

            // הגדרות ברירת מחדל לרישום חדש
            clientModel.PasswordHash = password; // הזרקת סיסמה
            clientModel.CreatedAt = DateTime.Now;
            clientModel.IsDeleted = false; // <--- כאן אנחנו קובעים שהוא "קיים" ופעיל

            // שמירה ב-Repository
            await _clientRepo.AddClientAsync(clientModel);
        }
        public async Task<ClientDto> LoginAsync(string email, string password)
        {
            var clients = await _clientRepo.GetAllClientsAsync();

            var client = clients.FirstOrDefault(c =>
                c.Email == email &&
                c.PasswordHash == password &&
                !c.IsDeleted); // בדיקה שהלקוח פעיל

            if (client == null) return null;

            return _mapper.Map<ClientDto>(client);
        }
    }
}