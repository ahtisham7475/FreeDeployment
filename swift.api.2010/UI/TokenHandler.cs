using swift.api.code.token;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

public class TokenHandler : IHttpHandler
{
    private readonly TokenResult checkResult;

    public TokenHandler(TokenResult checkResult)
        : base()
    {
        this.checkResult = checkResult;
    }

    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = Encoding.UTF8;

        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TokenResult));
        MemoryStream ms = new MemoryStream();
        ser.WriteObject(ms, checkResult);
        string jsonString = Encoding.UTF8.GetString(ms.ToArray());
        ms.Close();

        context.Response.Write(jsonString);
    }
}