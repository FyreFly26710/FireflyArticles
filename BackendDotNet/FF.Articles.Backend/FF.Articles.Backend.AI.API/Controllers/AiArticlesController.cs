using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles")]
public class AiArticlesController(
    IArticleGenerationService articleGenerationService,
    IContentsApiRemoteService contentsApiRemoteService,
    ITokenService tokenService,
    ILogger<AiArticlesController> logger) : ControllerBase
{

    [HttpPost("generate-article-list")]
    public async Task<ApiResponse<ArticlesAIResponseDto>> GenerateArticleList(ArticleListRequest request, CancellationToken cancellationToken)
    {
        var article = await articleGenerationService.GenerateArticleListsAsync(request, cancellationToken);
        return ResultUtil.Success(article);
    }

    // [HttpPost("generate-article-content")]
    // public async Task<IActionResult> GenerateArticleContent(ContentRequest request)
    // {
    //     var content = await articleGenerationService.GenerateArticleContentAsync(request, Request);
    //     return Ok(content);
    // }
    // [HttpPost("generate-article-content-batch")]
    // public async Task<IActionResult> BatchGenerateArticleContent(List<int> articleIds, CancellationToken cancellationToken)
    // {
    //     await articleGenerationService.BatchGenerateArticleContentAsync(articleIds, Request, cancellationToken);
    //     return Ok();
    // }

    /// <summary>
    /// Test endpoint that demonstrates JWT token authentication
    /// </summary>
    [HttpGet("test-auth")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public ApiResponse<TestDto> TestAuth()
    {
        // Get user from the request
        var userDto = new UserApiDto();

        if (UserUtil.TryGetUserFromHttpRequest(Request, out var user))
        {
            userDto = user;
            logger.LogInformation($"Request authenticated as user: {userDto.UserName}, role: {userDto.UserRole}");

            // Check authentication scheme
            var scheme = Request.HttpContext.User.Identity.AuthenticationType;

            return ResultUtil.Success(new TestDto
            {
                Message = "Authentication successful!",
                User = userDto.UserName,
                Role = userDto.UserRole,
                AuthType = scheme
            });
        }

        throw new Exception("Authentication failed");
    }

    /// <summary>
    /// Test endpoint for API-to-API communication using system admin user
    /// </summary>
    [HttpGet("test-api-communication")]
    public async Task<ApiResponse<TestDto>> TestApiCommunication()
    {
        try
        {
            // Create system admin user token for test
            var adminUser = new UserApiDto
            {
                UserId = -1,
                UserName = "deepseek",
                UserRole = UserConstant.ADMIN_ROLE,
                UserAccount = "system_api",
                CreateTime = DateTime.UtcNow
            };

            // Generate token for this admin user
            var token = tokenService.GenerateApiToken(adminUser);

            // Log the token for testing purposes
            logger.LogInformation($"Generated token: {token}");

            // Test API communication with Contents API
            var topicId = await contentsApiRemoteService.AddTopicByTitleAsync("Test Topic fro", Request);

            return ResultUtil.Success(new TestDto
            {
                Message = "API communication test successful!",
                Token = token,
                TopicId = topicId,
                SystemUser = adminUser.UserName
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during API communication test");
            throw ex;
        }
    }

}
public class TestDto
{
    public string Message { get; set; }
    public string User { get; set; }
    public string Role { get; set; }
    public string AuthType { get; set; }
    public string Token { get; set; }
    public long TopicId { get; set; }
    public string SystemUser { get; set; }
}
