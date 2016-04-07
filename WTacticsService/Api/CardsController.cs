using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Hangfire;
using WTacticsDAL;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Layout;
using WTacticsLibrary.Model;
using WTacticsService.Api.Authentication;

namespace WTacticsService.Api
{
    public class CardsController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string search = null)
        {
            using (var repository = new Repository())
            {

                IQueryable<CardModel> dbResult = repository.Context.Cards.Include(x=>x.Serie).Include(x=>x.Faction).Include(x=>x.Status).Include(x=>x.Type).Include(x => x.Creator).Include(x => x.LastModifiedBy).AsNoTracking();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    dbResult = dbResult.Where(it => it.Name.Contains(search) || it.Creator.Name.Contains(search));
                }

                var query = await dbResult.OrderByDescending(it => it.LastModifiedTime).Take(100).ToListAsync();
                var result = new ResultList<Card>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            using (var repository = new Repository())
            {
                var result = await repository.Context.Cards.FindByGuidAsync(id);
                await repository.Context.Entry(result).Reference(x=>x.Faction).LoadAsync();
                await repository.Context.Entry(result).Reference(x => x.Serie).LoadAsync();
                await repository.Context.Entry(result).Reference(x => x.Type).LoadAsync();
                await repository.Context.Entry(result).Reference(x => x.Status).LoadAsync();
                await repository.Context.Entry(result).Reference(x => x.Creator).LoadAsync();
                await repository.Context.Entry(result).Reference(x => x.LastModifiedBy).LoadAsync();

                return Request.CreateResponse(result.FromDal());
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Export(Guid id, ExportFormat format)
        {
            Repository.InitPaths();
            var cardPath = "";
            var mediaType = "image/png";
            switch (format)
            {
                case ExportFormat.Art:
                    cardPath = Repository.GetArtFile(id);
                    break;
                case ExportFormat.Png:
                    using (var repository = new Repository())
                    {
                        var result = await repository.Context.Cards.FindByGuidAsync(id);
                        if (!string.IsNullOrEmpty(result.PngCreationJobId))
                        {
                            // still busy creating png
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                    cardPath = Repository.GetHighResolutionPngFile(id);

                    break;
                case ExportFormat.BackgroundPng:
                    using (var repository = new Repository())
                    {
                        var result = await repository.Context.Cards.FindByGuidAsync(id);
                        await repository.Context.Entry(result).Reference(x => x.Faction).LoadAsync();
                        await repository.Context.Entry(result).Reference(x => x.Type).LoadAsync();
                        cardPath = Repository.GetBackgroundPngFile(result.Faction.Name, result.Type.Name);
                    }
                    break;
                case ExportFormat.BackPng:
                    cardPath = Repository.GetHighResolutionBackPngFile();
                    break;
                case ExportFormat.OverlaySvg:
                    cardPath = Repository.GetOverlaySvgFile(id);
                    mediaType = "image/svg+xml";
                    break;
                case ExportFormat.Svg:
                    cardPath = Repository.GetSvgFile(id);
                    mediaType = "image/svg+xml";
                    break;
                case ExportFormat.BackSvg:
                    cardPath = Repository.GetBackSvgFile();
                    mediaType = "image/svg+xml";
                    break;
            }

            if (!File.Exists(cardPath)) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The card with the specified id does not exist");

            Stream stream = new FileStream(cardPath, FileMode.Open);

            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            response.Content.Headers.ContentLength = stream.Length;
            return response;
        }


        /// <summary>
        /// Get all the cards for a user
        /// /api/Cards?userId=xx
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<HttpResponseMessage> GetForUser(Guid userId)
        {
            using (var repository = new Repository())
            {
                var query = await repository.Context.Cards.Where(x => x.Creator.Guid == userId).ToListAsync();
                var result = new ResultList<Card>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Card card)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {
                var cardModel = repository.CreateCard(card.Name, Guid.NewGuid());
                await repository.Context.Entry(cardModel.Type).Reference(x => x.TemplateInfo).LoadAsync();
                card = cardModel.FromDal();
                card.Type = cardModel.Type.FromDal(true);

                var cardGenerator = new CardGenerator(card);
                await cardGenerator.Generate();
                await cardGenerator.Generate(false);

                cardModel.PngCreationJobId = BackgroundJob.Schedule(() => CardGenerator.CreatePngJob(card.Guid), TimeSpan.FromMinutes(1));
                await repository.Context.SaveChangesAsync();

                return Request.CreateResponse(card);
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] Guid id)
        {
            var principal = HttpContext.Current.User as Principal;
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        [Authorize]
        [HttpPatch]
        public async Task<HttpResponseMessage> Patch([FromUri]Guid id, [FromBody] Card card)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {
                var cardModel = await repository.Context.Cards.FindByGuidAsync(id);

                await repository.Context.Entry(cardModel).Reference(x => x.Faction).LoadAsync();
                await repository.Context.Entry(cardModel).Reference(x => x.Serie).LoadAsync();
                await repository.Context.Entry(cardModel).Reference(x => x.Type).LoadAsync();
                await repository.Context.Entry(cardModel).Reference(x => x.Status).LoadAsync();

                var serieModel = await repository.Context.Series.FindByGuidAsync(card.Serie.Guid);
                var factionModel = await repository.Context.Factions.FindByGuidAsync(card.Faction.Guid);
                var cardTypeModel = await repository.Context.CardTypes.FindByGuidAsync(card.Type.Guid);
                var statusModel = await repository.Context.Statuses.FindByGuidAsync(card.Status.Guid);

                var hasLayoutChanges = false;

                if (card.Name != cardModel.Name) hasLayoutChanges = true;
                if (card.Faction.Guid != cardModel.Faction.Guid) hasLayoutChanges = true;
                if (card.Type.Guid != cardModel.Type.Guid) hasLayoutChanges = true;

                if (card.FirstName != cardModel.FirstName) hasLayoutChanges = true;
                if (card.LastName != cardModel.LastName) hasLayoutChanges = true;
                if (card.Artist != cardModel.Artist) hasLayoutChanges = true;
                if (card.SubType != cardModel.SubType) hasLayoutChanges = true;
                if (card.Cost != cardModel.Cost) hasLayoutChanges = true;
                if (card.Loyalty != cardModel.Loyalty) hasLayoutChanges = true;
                if (card.Attack != cardModel.Attack) hasLayoutChanges = true;
                if (card.Defense != cardModel.Defense) hasLayoutChanges = true;
                if (card.Info != cardModel.Info) hasLayoutChanges = true;
                if (card.LayoutText != cardModel.LayoutText) hasLayoutChanges = true;
                
                cardModel.Patch(card, serieModel, factionModel, cardTypeModel, statusModel, repository.ServiceUser);

               

                if (hasLayoutChanges)
                {
                    var jobId = cardModel.PngCreationJobId;
                    if (!string.IsNullOrEmpty(jobId))
                    {
                        BackgroundJob.Delete(jobId);
                    }

                    var cardGenerator = new CardGenerator(card);
                    await cardGenerator.Generate();
                    await cardGenerator.Generate(false);

                    //var cardWidthPx = 1535;
                    //var defaultDpi = 600;

                    // write low res png
                    // UlraHD (4k) 3840x2160 =>200.29 dpi
                    // fullHD 1920x1080 => 100.13 dpi
                    //var targetDpi = 100;
                    //var factor = (double)targetDpi/defaultDpi;
                    //var targetWith = (int) (cardWidthPx*factor);

                    cardModel.PngCreationJobId = BackgroundJob.Schedule(() => CardGenerator.CreatePngJob(card.Guid), TimeSpan.FromMinutes(1));

                }
                await repository.Context.SaveChangesAsync();

                return Request.CreateResponse(cardModel.FromDal());
            }
        }

       

    }
}
