using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Infrastructure;

public class AssetFunction
{
    private readonly IBlobStorageService _blobs;

    public AssetFunction(IBlobStorageService blobs)
    {
        _blobs = blobs;
    }

    [Function("UploadAsset")]
    public async Task<HttpResponseData> Upload([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assets")] HttpRequestData req)
    {
        using var ms = new MemoryStream();
        await req.Body.CopyToAsync(ms);
        ms.Position = 0;

        var id = Guid.NewGuid().ToString();
        await _blobs.UploadAsync("assets", id, ms, req.FunctionContext.CancellationToken);
        var uri = await _blobs.GetReadUriAsync("assets", id, TimeSpan.FromHours(1), req.FunctionContext.CancellationToken);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteStringAsync(uri.ToString());
        return res;
    }
}
