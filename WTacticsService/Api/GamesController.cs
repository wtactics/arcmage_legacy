using System.Net.Http;
using System.Web.Http;
using WTacticsService.Api.GameRuntime;

namespace WTacticsService.Api
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
