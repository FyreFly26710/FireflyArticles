using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.AI.API.Models.Requests.ChatRounds;
using FF.Articles.Backend.Common.ApiDtos;
namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface IChatRoundService : IBaseService<ChatRound>
{
    Task<ChatRoundDto> NewChatRound(ChatRoundCreateRequest request, UserApiDto user, CancellationToken cancellationToken = default);
    // Task<ChatRoundDto> ReQueryChatRound(ChatRoundReQueryRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default);
    IAsyncEnumerable<SseDto> StreamChatRound(ChatRoundCreateRequest request, UserApiDto user, CancellationToken cancellationToken = default);
    Task<bool> DisableChatRound(List<long> ids);
    Task<bool> EnableChatRound(List<long> ids);
    Task<bool> DeleteAsync(List<long> ids);
    Task<long> GetUserIdByChatRoundId(List<long> ids);
}
