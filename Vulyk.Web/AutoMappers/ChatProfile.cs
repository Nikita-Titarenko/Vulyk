using AutoMapper;
using Vulyk.Application.DTOs.Chat;
using Vulyk.Web.ViewModels.Chat;

namespace Vulyk.Web.AutoMappers
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
