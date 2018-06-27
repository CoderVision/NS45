using NtccSteward.Core.Services;
using NtccSteward.Repository.Import;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NtccSteward.Api.Controllers
{
   // [Authorize]
    public class UploadsController : ApiController
    {
        private readonly IImportService importService;

        public UploadsController(IImportService importService)
        {
            this.importService = importService;
        }

        [Route("uploads")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadFile()
        {
            // Note:  Might be able to remove churchId!  Just add the church from the ChurchInfo table

            // https://stackoverflow.com/questions/10320232/how-to-accept-a-file-post
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            // Reference this for large file uploads:
            // https://www.strathweb.com/2012/09/dealing-with-large-files-in-asp-net-web-api/
            var files = await Request.Content.ReadAsMultipartAsync(provider);

            var localFilePath = provider.FileData.First().LocalFileName;

            System.IO.File.Move(localFilePath, $"{localFilePath}.mdb");

            localFilePath += ".mdb";
            
            this.importService.ImportMdbFile(localFilePath );

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
