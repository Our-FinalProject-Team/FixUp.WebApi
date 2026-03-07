using AutoMapper; // אם את משתמשת ב-AutoMapper, אם לא - נעשה המרה ידנית
using FixUp.Repository.Models;
using FixUp.Repository.Repositories;
using FixUp.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixUp.Service.DTOs;
using FixUp.Service.Interfaces;
using FixUp.Repository.Interfaces;
using FixUp.Repository.Models;
using YourProjectName.Models;
using Microsoft.Extensions.Configuration;

namespace FixUp.Service.Services
{
    
        public class MessageService : IMessageService
        {
            private readonly IChatRepository _repository;
            //IConfiguration   זהו ממשק של מיקרוסופט
            // שיודע לקרוא נתונים מקובץ appsettings.json 
            //ומבצע שינויים באופן דינאמי
            private readonly IConfiguration _configuration;

            public MessageService(IChatRepository repository, IConfiguration configuration)
            {
                _repository = repository;
                _configuration = configuration;
            }

            /// <summary>
            /// סורק את תוכן ההודעה ומזהה את הקטגוריה המקצועית המתאימה 
            /// על בסיס מילות מפתח ב-4 שפות מתוך קובץ ההגדרות.
            /// </summary>
            /// <param name="content">תוכן ההודעה שנשלחה מהלקוח</param>
            /// <returns>מזהה קטגוריה (ID) או 0 אם לא נמצאה התאמה</returns>
            public int DetectCategoryId(string content)
            {
                if (string.IsNullOrWhiteSpace(content)) return 0;

                // ניקוי סימני פיסוק לזיהוי מדויק יותר
                string cleanContent = new string(content.Where(c => !char.IsPunctuation(c)).ToArray());

                //העברה למילון כדי שיהיה נוח לעבור על הנתונים
                //במילון המפתח הוא הקטגוריה
                //הערך זה מילון שבתוכו יש שפה ורשימה של מילים בשפה הזו שקשורים לקטגוריה
                //
                var categories = new Dictionary<string, Dictionary<string, List<string>>>();
                _configuration.GetSection("ProfessionalSettings:Categories").Bind(categories);

                if (categories == null) return 0;

                foreach (var categoryEntry in categories)
                {
                    foreach (var languageEntry in categoryEntry.Value)
                    {
                        // דילוג על שדה ה-Name ב-JSON שאינו רשימת מילים
                        if (languageEntry.Key == "Name") continue;

                        if (languageEntry.Value.Any(word => cleanContent.Contains(word, StringComparison.OrdinalIgnoreCase)))
                        {
                            return int.Parse(categoryEntry.Key);
                        }
                    }
                }
                return 0;
            }

            public async Task AddAsync(MessageDTO item)
            {
                int detectedId = DetectCategoryId(item.Content);

                var model = new Message
                {
                    Content = item.Content,
                    CreatedAt = DateTime.Now,
                    SenderId = item.SenderId,
                    SenderName = item.SenderName,
                    SenderRole = item.SenderRole,
                    // השמה של הקטגוריה שזוהתה או מה שנשלח במקור
                    CategoryId = detectedId != 0 ? detectedId : item.CategoryId,
                    ImageUrl = item.ImageUrl
                };

                await _repository.AddMessageAsync(model);
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

            public async Task<IEnumerable<MessageDTO>> GetByCategoryIdAsync(int categoryId)
            {
                var messages = await _repository.GetMessagesByCategoryAsync(categoryId);
                return messages.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    SenderName = m.SenderName,
                    SenderRole = m.SenderRole,
                    CategoryId = m.CategoryId
                });
            }

            // מימושים ריקים לבינתיים
            public Task<MessageDTO> GetByIdAsync(int id) => throw new NotImplementedException();
            public Task UpdateAsync(int id, MessageDTO item) => throw new NotImplementedException();
            public Task DeleteAsync(int id) => throw new NotImplementedException();
        }
    

}