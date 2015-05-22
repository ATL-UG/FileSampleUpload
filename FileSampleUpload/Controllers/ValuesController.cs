using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FileSampleUpload.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public async Task<HttpResponseMessage> Post()
        {
            //pick file with name: file
            HttpPostedFile uploadedFile = HttpContext.Current.Request.Files["file"];
            if (uploadedFile == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            //retrieve the string with name value...
            String value = HttpContext.Current.Request.Form["value"] ?? ""; //adding empty string incase no content was recieved...

            //validate and save/process the file as you wish...


            //return positive respnse...
            return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, String.Format("Successfully Uploaded File. String recieved: {0}", value)));
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

//        [Route("fileUpload")]
//        [HttpPost]
//        public async Task<HttpResponseMessage> FileUpload()
//        {
//            //pick file with name: file
//            HttpPostedFile uploadedFile = HttpContext.Current.Request.Files["file"];
//            if (uploadedFile == null)
//            {
//                throw new HttpResponseException(HttpStatusCode.BadRequest);
//            }
//
//            //validate and save/process the file as you wish...
//
//
//            //return positive respnse...
//            return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, "Successfully Uploaded File"));
//
//        } 
    }
}
