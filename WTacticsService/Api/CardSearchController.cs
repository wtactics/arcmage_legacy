using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WTacticsDAL;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Model;
using WTacticsService.Api.Authentication;

namespace WTacticsService.Api
{
    public class CardSearchController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] CardSearchOptions searchOptionsBase)
        {
            var token = TokenExtracton.GetTokenFromCookie(HttpContext.Current.Request);
            using (var repository = new Repository(token))
            {
             
                IQueryable<CardModel> dbResult = repository.Context.Cards.Include(x => x.RuleSet).Include(x => x.Serie).Include(x => x.Faction).Include(x => x.Status).Include(x => x.Type).Include(x => x.Creator).Include(x => x.LastModifiedBy).AsNoTracking();

                if (!searchOptionsBase.ShowDraftVersions)
                {
                    dbResult = dbResult.Where(x => x.Status.Guid == PredefinedGuids.Final);
                }

                

                if (!string.IsNullOrWhiteSpace(searchOptionsBase.Search))
                {
                    dbResult = dbResult.Where(it => it.Name.Contains(searchOptionsBase.Search) || it.Creator.Name.Contains(searchOptionsBase.Search));
                }
                var totalCount = dbResult.Count();

                if (QueryHelper.PropertyExists<CardModel>(searchOptionsBase.OrderBy))
                {
                    var orderByExpression = QueryHelper.GetPropertyExpression<CardModel>(searchOptionsBase.OrderBy);
                    dbResult = dbResult.OrderBy(orderByExpression);
                }
                else
                {
                    dbResult = dbResult.OrderByDescending(it => it.LastModifiedTime);
                }


                searchOptionsBase.PageSize = Math.Min(50, searchOptionsBase.PageSize);

                var query = await dbResult.Skip((searchOptionsBase.PageNumber - 1) * searchOptionsBase.PageSize).Take(searchOptionsBase.PageSize).ToListAsync();

                var result = new ResultList<Card>(query.Select(x => x.FromDal()).ToList()) { TotalItems = totalCount, SearchOptions = searchOptionsBase };
                return Request.CreateResponse(result);
            }
        }
    }
}
