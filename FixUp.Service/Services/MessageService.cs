using AutoMapper; // אם את משתמשת ב-AutoMapper, אם לא - נעשה המרה ידנית
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.DTOs;


using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using YourProjectName.Models;

namespace FixUp.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly IChatRepository _repository;

        public MessageService(IChatRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MessageDTO>> GetAllAsync()
        {
            var messages = await _repository.GetAllMessagesAsync();
            return messages.Select(m => new MessageDTO
            {
                Id = m.Id,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                SenderId = m.SenderId,
                SenderName = m.SenderName,
                SenderRole = m.SenderRole,
                CategoryId = m.CategoryId,
                ImageUrl = m.ImageUrl

            });
        }

        public async Task AddAsync(MessageDTO item)
        {
            var model = new Message
            {
                Content = item.Content,
                CreatedAt = DateTime.Now,
                SenderId = item.SenderId,
                SenderName = item.SenderName,
                SenderRole = item.SenderRole,
                CategoryId = item.CategoryId,
                ImageUrl = item.ImageUrl
            };
            await _repository.AddMessageAsync(model);
        }

        public async Task<IEnumerable<MessageDTO>> GetByCategoryIdAsync(int categoryId)
        {
            var messages = await _repository.GetMessagesByCategoryAsync(categoryId);

            // אם המערך חוזר ריק, סימן שב-DB אין הודעות עם CategoryId = 3
            return messages.Select(m => new MessageDTO
            {
                Id = m.Id,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                SenderName = m.SenderName,
                SenderRole = m.SenderRole,
                CategoryId = m.CategoryId,
                ImageUrl = m.ImageUrl
            });
        }

        // מימוש שאר הפונקציות של IService (אפשר להשאיר ריק או לזרוק NotImplEx)
        public Task<MessageDTO> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task UpdateAsync(int id, MessageDTO item) => throw new NotImplementedException();
        public Task DeleteAsync(int id) => throw new NotImplementedException();
    }
}