using AutoMapper;
using Vulyk.Application.DTOs.Message;

namespace Vulyk.Application.AutoMappers
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Domain.Models.Message, MessageListItemDto>();
            CreateMap<IEnumerable<Domain.Models.Message>, MessageListDto>().ForMember(dto => dto.Messages, model => model.MapFrom(model => model));
        }
    }
}
