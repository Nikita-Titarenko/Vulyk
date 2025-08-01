using AutoMapper;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.ViewModels;

namespace Vulyk.AutoMappers
{
    public class MessageProfile : Profile
    {
        public MessageProfile() {
            CreateMap<MessageListDto, MessageListViewModel>();
            CreateMap<MessageListItemDto, MessageListItemViewModel>();
        }
    }
}