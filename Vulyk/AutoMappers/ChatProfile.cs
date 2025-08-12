using AutoMapper;
using Vulyk.DTOs;
using Vulyk.ViewModels;

namespace Vulyk.AutoMappers
{
    public class ChatProfile : Profile
    {
        public ChatProfile() {
            CreateMap<ChatListItemDto, ChatListItemViewModel>();
            CreateMap<ChatListDto, ChatListViewModel>();
            CreateMap<GetUserChatResultDto, CreateUserChatResultDto>();
        }
    }
}
