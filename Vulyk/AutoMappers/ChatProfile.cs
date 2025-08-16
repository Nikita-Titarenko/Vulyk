using AutoMapper;
using Vulyk.DTOs.Chat;
using Vulyk.ViewModels.Chat;

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
