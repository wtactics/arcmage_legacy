using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WTacticsGameService.Api.GameRuntime;

namespace WTacticsGameService.Api
{
    
    public class GamesController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Game game)
        {
            var createdGame = GameRepository.Instance.CreateGame(game.Name);
            return Request.CreateResponse(createdGame);
        }

    }
}
