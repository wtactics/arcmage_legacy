using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WTacticsDAL;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Model;

namespace WTacticsService.Api
{
    public class DeckSearchController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] SearchOptionsBase searchOptionsBase)
        {
            using (var repository = new Repository())
            {
                IQueryable<DeckModel> dbResult = repository.Context.Decks.Include(x => x.Creator).AsNoTracking();

                if (!string.IsNullOrWhiteSpace(searchOptionsBase.Search))
                {
                    dbResult =
                        dbResult.Where(
                            it =>
                                it.Name.Contains(searchOptionsBase.Search) ||
                                it.Creator.Name.Contains(searchOptionsBase.Search));
                }
                var totalCount = dbResult.Count();

                if (QueryHelper.PropertyExists<DeckModel>(searchOptionsBase.OrderBy))
                {
                    var orderByExpression = QueryHelper.GetPropertyExpression<DeckModel>(searchOptionsBase.OrderBy);
                    dbResult = dbResult.OrderBy(orderByExpression);
                }
                else
                {
                    dbResult = dbResult.OrderByDescending(it => it.LastModifiedTime);
                }


                searchOptionsBase.PageSize = Math.Min(50, searchOptionsBase.PageSize);

                var query =
                    await
                        dbResult.Skip((searchOptionsBase.PageNumber - 1)*searchOptionsBase.PageSize)
                            .Take(searchOptionsBase.PageSize)
                            .ToListAsync();

                var result = new ResultList<Deck>(query.Select(x => x.FromDal()).ToList())
                {
                    TotalItems = totalCount,
                    SearchOptions = searchOptionsBase
                };
                return Request.CreateResponse(result);
            }
        }
    }
}
