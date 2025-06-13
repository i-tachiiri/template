using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Security.Claims;

public class MeFunction
{
    [Function("Me")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "me")] HttpRequestData req, FunctionContext context)
    {
        if (context.Items.TryGetValue("User", out var principalObj) && principalObj is ClaimsPrincipal user)
        {
            var res = req.CreateResponse(HttpStatusCode.OK);
            res.Headers.Add("Content-Type", "application/json");
            res.WriteString($"{{ \"name\": \"{user.Identity?.Name}\" }}");
            return res;
        }

        var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
        return unauthorized;
    }
}
