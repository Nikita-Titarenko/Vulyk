using AutoMapper;
using Vulyk.DTOs.Message;
using Vulyk.ViewModels.Message;

namespace Vulyk.AutoMappers
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<MessageListDto, MessageListViewModel>();
            CreateMap<MessageListItemDto, MessageListItemViewModel>();
            CreateMap<CreateMessageViewModel, CreateMessageDto>();
        }
    }
}