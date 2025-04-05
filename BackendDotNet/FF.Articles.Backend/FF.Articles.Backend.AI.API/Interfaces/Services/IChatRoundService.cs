using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.Chats;

namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface IChatRoundService : IBaseService<ChatRound>
{
    Task<ChatRoundDto> NewChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default);
    Task<ChatRoundDto> ReQueryChatRound(ChatRoundEditRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default);
}
