using API.Source.Model.Entity;
using API.Source.Modules.ChatMessage.Dto;
using API.Source.Modules.User.Dto;
using AutoMapper;

namespace API.Source.Config;

public class AutoMapperProfileConfig : Profile
{
    public AutoMapperProfileConfig()
    {
        CreateMap<User, GetUserDto>();
        CreateMap<User, UserDto>();
        CreateMap<ChatMessage, ChatMessageDto>();
    }
}