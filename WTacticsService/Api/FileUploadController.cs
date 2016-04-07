using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WTacticsLibrary;

namespace WTacticsService.Api
{
    [Authorize]
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post(Guid id)
        {
            Repository.InitPaths();
            var artFile = Repository.GetArtFile(id);
            if (File.Exists(artFile))
            {
                try
                {
                    File.Delete(artFile);
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }


            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var file = provider.Contents.FirstOrDefault();

            var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
            var stream = await file.ReadAsStreamAsync();
       

            
            using (var fileStream = File.Create(artFile))
            {
                try
                {
                    await stream.CopyToAsync(fileStream);
                    return Request.CreateResponse(HttpStatusCode.OK) ;
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
