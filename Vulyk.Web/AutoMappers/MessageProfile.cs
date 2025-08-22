using AutoMapper;
using Vulyk.Application.DTOs.Message;
using Vulyk.Web.ViewModels.Message;

namespace Vulyk.Web.AutoMappers
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