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
    public class UploadsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Test()
        {
            return Ok("Test Response");
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadFile()
        {
            // https://stackoverflow.com/questions/10320232/how-to-accept-a-file-post
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string uploadsFolder = System.Web.HttpContext.Current.Server.MapPath("~/uploads");
            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Reference this for large file uploads:
                // https://www.strathweb.com/2012/09/dealing-with-large-files-in-asp-net-web-api/
                var files = await Request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception ex)
            {
                var x = ex;
            }

             

            return await Task.FromResult(new HttpResponseMessage());

            //var task = request.Content.ReadAsMultipartAsync(provider).
            //    ContinueWith<HttpResponseMessage>(o =>
            //    {
            //        string file1 = provider.FileData.First().LocalFileName;
            //        // this is the file name on the server where the file was saved 
                    
            //        var fileName = Path.GetFileName(file1);

            //        File.Move(file1, $"{uploadsFolder}\\{fileName}");

            //        return new HttpResponseMessage() { Content = new StringContent("File uploaded.") };
            //    }
            //);
            //return task;
        }
    }
}
