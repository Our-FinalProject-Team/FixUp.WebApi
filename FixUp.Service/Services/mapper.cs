using AutoMapper;
using FixUp.Repository.Models;
using FixUp.Service.Dto;

namespace FixUp.Service.Services
{
    public class MyMapper : Profile
    {
        public MyMapper()
        {
            // מיפוי יוזר (כולל קליינט ופרופשיונל אם יצרת להם DTO נפרד)
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Client, ClientDto>().ReverseMap();
            CreateMap<Professional, ProfessionalDto>().ReverseMap();

            // מיפוי קטגוריות
            CreateMap<Category, CategoryDto>().ReverseMap();

            // מיפוי משימות
            CreateMap<FixUpTask, FixUpTaskDto>().ReverseMap();

            // אם יש לך עוד טבלאות כמו Review, פשוט הוסיפי שורה כזו לכל אחת
            CreateMap<Review, ReviewDto>().ReverseMap();
            
            CreateMap<RequestCreateDto, Request>(); // מיצירה למודל
            CreateMap<Request, RequestDisplayDto>(); // מהמודל לתצוגה
        }
    }
}