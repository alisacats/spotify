using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyBot.Host.Api.Model;

namespace SpotifyBot.Host.Api
{
    
    [ApiController]
    [Route("/accounts")]
    public sealed class CommonController : ControllerBase
    {
        [Route("get-brief-info")]
        public async Task<GetBriefInfoResponse> GetBriefInfo([FromServices] SpotifyServiceGroup spotifyServiceGroup)
        {
            var briefInfo = await spotifyServiceGroup.GetBriefInfo();
            return new GetBriefInfoResponse(){ Accounts = briefInfo };
        }
    }
}