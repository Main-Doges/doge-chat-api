using API.Source.Exception.Http;
using API.Source.Extension;
using API.Source.Modules.ChatMessage.Dto;
using API.Source.Modules.ChatMessage.Interfaces;
using API.Source.Modules.User.Interfaces;
using API.Source.SignalR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace API.Source.Modules.ChatMessage;

[ApiController]
[Route("[controller]")]
public class ChatMessageController : ControllerBase
{
    private readonly IChatMessageService _chatMessageService;
    private readonly IMainHubService _mainHubService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public ChatMessageController(
        IChatMessageService chatMessageService,
        IMainHubService mainHubService,
        IUserService userService,
        IMapper mapper
    )
    {
        _chatMessageService = chatMessageService;
        _mainHubService = mainHubService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet("{id:long}")]
    public async Task<dynamic> SendMessage([FromRoute] long id)
    {
        var userId = User.GetUserId();

        // validate other user exists
        var user = await _userService.GetUserById(id);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        var result = await _chatMessageService.GetMessages(userId, user.Id);

        return _mapper.Map<List<ChatMessageDto>>(result);
    }

    [HttpPost("send")]
    public async Task<ChatMessageDto> SendMessage([FromBody] SendChatMessageDto sendChatMessageDto)
    {
        var userId = User.GetUserId();

        // validate and save in database
        var result = await _chatMessageService.SaveMessage(userId, sendChatMessageDto);
        var chatMessageDto = _mapper.Map<ChatMessageDto>(result.ChatMessage);

        // notify receiver through socket
        await _mainHubService.SendMessage(result.ReceivedUser.Username, chatMessageDto);

        return chatMessageDto;
    }
}