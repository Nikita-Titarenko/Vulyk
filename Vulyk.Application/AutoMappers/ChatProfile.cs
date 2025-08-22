using AutoMapper;
using Vulyk.Application.DTOs.Chat;
using Vulyk.Domain.Models;

namespace Vulyk.Application.AutoMappers
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Chat, GetUserChatResultDto>().ForMember(dto => dto.ChatId, model => model.MapFrom(m => m.Id));
        }
    }
}
