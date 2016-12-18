using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WTacticsLibrary;
using WTacticsLibrary.Model;
using System.Web.Http.Cors;
using WTacticsGameService.Api.GameRuntime;

namespace WTacticsGameService.Api
{
    
    public class GameSearchController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] SearchOptionsBase searchOptionsBase)
        {
            var dbResult = GameRepository.Instance.GetGames().AsQueryable();
              
            if (!string.IsNullOrWhiteSpace(searchOptionsBase.Search))
            {
                dbResult = dbResult.Where(it => it.Name.Contains(searchOptionsBase.Search) );
            }
            var totalCount = dbResult.Count();

            // default order by
            if (string.IsNullOrWhiteSpace(searchOptionsBase.OrderBy))
            {
                searchOptionsBase.OrderBy = "Name";
            }

            var orderByType = QueryHelper.GetPropertyType<Game>(searchOptionsBase.OrderBy);
            if (orderByType != null)
            {
                if (orderByType == typeof(string))
                {
                    var orderByExpression = QueryHelper.GetPropertyExpression<Game, string>(searchOptionsBase.OrderBy);
                    dbResult = searchOptionsBase.ReverseOrder ? dbResult.OrderByDescending(orderByExpression) : dbResult.OrderBy(orderByExpression);
                }
                if (orderByType == typeof(int))
                {
                    var orderByExpression = QueryHelper.GetPropertyExpression<Game, int>(searchOptionsBase.OrderBy);
                    dbResult = searchOptionsBase.ReverseOrder ? dbResult.OrderByDescending(orderByExpression) : dbResult.OrderBy(orderByExpression);
                }
                if (orderByType == typeof(DateTime))
                {
                    var orderByExpression = QueryHelper.GetPropertyExpression<Game, DateTime>(searchOptionsBase.OrderBy);
                    dbResult = searchOptionsBase.ReverseOrder ? dbResult.OrderByDescending(orderByExpression) : dbResult.OrderBy(orderByExpression);
                }
            }

            searchOptionsBase.PageSize = Math.Min(50, searchOptionsBase.PageSize);
            var query = dbResult.Skip((searchOptionsBase.PageNumber - 1) * searchOptionsBase.PageSize).Take(searchOptionsBase.PageSize);
            var result = new ResultList<Game>(query.ToList()) { TotalItems = totalCount, SearchOptions = searchOptionsBase };
            return Request.CreateResponse(result);
        }
    }
}
