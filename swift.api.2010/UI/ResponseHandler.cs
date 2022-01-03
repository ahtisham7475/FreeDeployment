using swift.api.code;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace swift.api.ui
{
    // This class is responsible for handeling the return response
    //  The class gets the response from the logic and returns it to the user
    //  as a formatted JSON string
    public class ResponseHandler : IHttpHandler
    {

        private readonly OutputResult result;

        public ResponseHandler(OutputResult checkResult)
            : base()
        {
            result = checkResult;
        }

        // doesn't cache
        public bool IsReusable { get { return false; } }


        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            string json;

            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(OutputResult));
                ser.WriteObject(ms, result);
                json = Encoding.UTF8.GetString(ms.ToArray());
            }
            context.Response.Write(json);

        }
    }
}