using AutoMapper;
using Vulyk.Application.DTOs.Message;
using Vulyk.Web.ApiModels.Requests;
using Vulyk.Web.ApiModels.Responds;
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

            CreateMap<MessageListDto, MessageListResponseModel>();
            CreateMap<MessageListItemDto, MessageListItemResponseModel>();
            CreateMap<CreateMessageRequestModel, CreateMessageDto>();
        }
    }
}